using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class KeyValueContentController : Controller
    {
        #region Properties and fields

        private IKeyValueContentService _keyValueContentService { get; }
        private ILogService _logService { get; }

        private List<KeyValueContent> _allEntities { get; set; }
        private KeyValueContentModel _model;
        private KeyValueContent _entity;

        #endregion


        #region Constructor

        public KeyValueContentController(IKeyValueContentService KeyValueContentService, ILogService logService)
        {
            _keyValueContentService = KeyValueContentService;
            _logService = logService;

            _allEntities = new List<KeyValueContent>();
            _model = new KeyValueContentModel();
            _entity = new KeyValueContent();
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
            List<KeyValueContentModel> convertedEntityList = new List<KeyValueContentModel>();

            _allEntities = await _keyValueContentService.ListAsync();

            List<KeyValueContent> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new KeyValueContentModel();
                EntityToModel(entity, ref _model, index);

                convertedEntityList.Add(_model);

                index++;
            }

            ViewBag.NumberOfPages = Math.Ceiling((decimal)_allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            _entity = await _keyValueContentService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            EmptyModel(ref _model);

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(KeyValueContentModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);
                await _keyValueContentService.CreateAsync(_entity);

                await Log(LogType.ContentAdd, "Create", $"{_entity.Id}_ {_entity.ContentKey}");
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
                _entity = await _keyValueContentService.FindByIdAsync(id);
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
        public async Task<ActionResult> Update(KeyValueContentModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);
                await _keyValueContentService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.ContentKey}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "KeyValueContent");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<KeyValueContentModel> convertedEntityList = new List<KeyValueContentModel>();
            List<KeyValueContent> removedEntityList = await _keyValueContentService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new KeyValueContentModel();
                EntityToModel(entity, ref _model, index);

                convertedEntityList.Add(_model);

                index++;
            }


            return PartialView("_TrashList" ,convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _keyValueContentService.DeleteAsync(id);

                await Log(LogType.ContentDelete, "Delete", $"{id}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> Remove(int id)
        {
            try
            {
                await _keyValueContentService.RemoveAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> Restore(int id)
        {
            try
            {
                await _keyValueContentService.RestoreAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteAllTrash()
        {
            try
            {
                await _keyValueContentService.DeleteRangeAsync(await _keyValueContentService.ListOfRemovedAsync());
                await Log(LogType.ContentAdd, "DeleteAllTrash", $"Deleted all items in the trash");
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

        private void EntityToModel(KeyValueContent entity, ref KeyValueContentModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.ContentKey= entity.ContentKey;
            model.ContentValue= entity.ContentValue;
            model.Description= entity.Description;
            model.ContentType = entity.ContentType;

            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(KeyValueContent entity, ref KeyValueContentModel model, int index)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.index = index;
            model.ContentKey = entity.ContentKey;
            model.ContentValue = entity.ContentValue;
            model.Description = entity.Description;
            model.ContentType = entity.ContentType;

            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void ModelToEntity(KeyValueContentModel model, ref KeyValueContent entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.ContentKey = model.ContentKey;
            entity.ContentValue = model.ContentValue;
            entity.Description = model.Description;
            entity.ContentType = model.ContentType;
        }

        private void EmptyEntity(ref KeyValueContent entity)
        {
            entity.Id = 0;
            entity.ContentKey = "";
            entity.ContentValue = "";
            entity.Description = "";
            entity.ContentType = KeyValueContentType.Text;
            entity.UpdateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref KeyValueContentModel model)
        {
            model.Id = 0;
            model.index = 0;
            model.ContentKey = "";
            model.ContentValue = "";
            model.Description = "";
            model.ContentType = KeyValueContentType.Text;
            model.UpdateDateTime = DateTime.UtcNow;
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