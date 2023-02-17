using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using File = Dokan.Domain.Website.File;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class FileController : ManagementBaseController
    {
        #region Properties and fields

        private IFileService _fileService { get; }
        private ILogService _logService { get; }

        private List<File> _allEntities { get; set; }
        private FileModel _model;
        private File _entity;

        #endregion


        #region Constructor

        public FileController(IFileService FileService, ILogService logService)
        {
            _fileService = FileService;
            _logService = logService;

            _allEntities = new List<File>();
            _model = new FileModel();
            _entity = new File();
        }

        #endregion


        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> List(int page = 1, int numberOfResults = 5)
        {
            List<FileModel> convertedEntityList = new List<FileModel>();

            _allEntities = await _fileService.ListAsync();

            List<File> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new FileModel();
                EntityToModel(entity, ref _model, index);

                convertedEntityList.Add(_model);

                index++;
            }

            convertedEntityList = convertedEntityList.OrderBy(x => x.FileType).ThenBy(x => x.CreateDateTime).ToList();

            ViewBag.NumberOfPages = Math.Ceiling((decimal)_allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            _entity = await _fileService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            EmptyModel(ref _model);

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(FileModel model, HttpPostedFileBase uploadedFile)
        {
            if (!ModelState.IsValid || uploadedFile is null)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);

                string fileFormat = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.'));

                _entity.Title += fileFormat;

                //model.Title = Path.GetFileName(uploadedFile.FileName);

                _entity.CreateDateTime = DateTime.UtcNow;

                await _fileService.CreateAsync(_entity);

                string path = Path.Combine(Server.MapPath("~/Files"), _entity.Title);
                uploadedFile.SaveAs(path);

                await Log(LogType.ContentAdd, "Create", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            try
            {
                _entity = await _fileService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            EntityToModel(_entity, ref _model);

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(FileModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _entity = await _fileService.FindByIdAsync(model.Id);

                // Change the name of the saved file if title was edited
                if (_entity.Title != model.Title)
                {
                    string oldPath = Path.Combine(Server.MapPath("~/Files"), _entity.Title);
                    string newPath = Path.Combine(Server.MapPath("~/Files"), model.Title);
                    System.IO.File.Move(oldPath, newPath);
                }

                ModelToEntity(model, ref _entity);
                await _fileService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "File");
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _entity = await _fileService.FindByIdAsync(id);
                await _fileService.DeleteAsync(id);

                string path = Path.Combine(Server.MapPath("~/Files"), _entity.Title);
                System.IO.File.Delete(path);

                await Log(LogType.ContentDelete, "Delete", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #endregion


        #region Conversion Methods

        private void EntityToModel(File entity, ref FileModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.FileType= entity.FileType;

            model.CreateDateTime = entity.CreateDateTime;
        }

        private void EntityToModel(File entity, ref FileModel model, int index)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Index = index;
            model.Title = entity.Title;
            model.FileType = entity.FileType;

            model.CreateDateTime = entity.CreateDateTime;
        }

        private void ModelToEntity(FileModel model, ref File entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title= model.Title;
            entity.FileType = model.FileType;
            entity.CreateDateTime = model.CreateDateTime;
        }

        private void EmptyEntity(ref File entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.FileType = FileType.Other;
            entity.CreateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref FileModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.Title = "";
            model.FileType = FileType.Other;
            model.CreateDateTime = DateTime.UtcNow;
        }

        #endregion


        #region Log and Preperation Methods

        private async Task LogError(Exception ex)
        {
            await _logService.CreateAsync(new Log()
            {
                LogType = LogType.Error,
                Controller = ex.Source,
                Method = ex.TargetSite.Name,
                Description = ex.Message,
                AdditionalInfo = ex.InnerException.ToString(),
                Code = ex.HResult.ToString(),
            });
        }

        private async Task Log(LogType logType, string method, string description)
        {
            await _logService.CreateAsync(new Log()
            {
                LogType = logType,
                Controller = this.GetType().Name,
                Method = method,
                Description = $"{logType} _ {description}",
            });
        }
        
        #endregion
    }
}