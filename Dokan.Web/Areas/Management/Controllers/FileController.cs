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

        #endregion


        #region Constructor

        public FileController(IFileService FileService, ILogService logService)
        {
            _fileService = FileService;
            _logService = logService;
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

            var allEntities = await _fileService.ListAsync();

            List<File> filteredList = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                var model = FileModel.EntityToModel(in entity, index);

                convertedEntityList.Add(model);

                index++;
            }

            convertedEntityList = convertedEntityList.OrderBy(x => x.FileType).ThenBy(x => x.CreateDateTime).ToList();

            ViewBag.NumberOfPages = Math.Ceiling((decimal)allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var entity = await _fileService.FindByIdAsync(id);

            var model = FileModel.EntityToModel(in entity);

            return PartialView("_Details", model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new FileModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(FileModel model, HttpPostedFileBase uploadedFile)
        {
            if (!ModelState.IsValid || uploadedFile is null)
                return View(model);

            try
            {
                var entity = FileModel.ModelToEntity(in model);

                string fileFormat = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.'));

                entity.Title += fileFormat;

                //model.Title = Path.GetFileName(uploadedFile.FileName);

                entity.CreateDateTime = DateTime.UtcNow;

                await _fileService.CreateAsync(entity);

                string path = Path.Combine(Server.MapPath("~/Files"), entity.Title);
                uploadedFile.SaveAs(path);

                await Log(LogType.ContentAdd, "Create", $"{entity.Id}_ {entity.Title}");
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
            var entity = new File();
            try
            {
                entity = await _fileService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            var model = FileModel.EntityToModel(in entity);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(FileModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var entity = await _fileService.FindByIdAsync(model.Id);

                // Change the name of the saved file if title was edited
                if (entity.Title != model.Title)
                {
                    string oldPath = Path.Combine(Server.MapPath("~/Files"), entity.Title);
                    string newPath = Path.Combine(Server.MapPath("~/Files"), model.Title);
                    System.IO.File.Move(oldPath, newPath);
                }

                entity = FileModel.ModelToEntity(in model);
                await _fileService.UpdateAsync(entity);

                await Log(LogType.ContentUpdate, "Update", $"{entity.Id}_ {entity.Title}");
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
                var entity = await _fileService.FindByIdAsync(id);
                await _fileService.DeleteAsync(id);

                string path = Path.Combine(Server.MapPath("~/Files"), entity.Title);
                System.IO.File.Delete(path);

                await Log(LogType.ContentDelete, "Delete", $"{entity.Id}_ {entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #endregion


        #region Log Methods

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