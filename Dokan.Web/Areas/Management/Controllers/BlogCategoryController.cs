using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class BlogCategoryController : Controller
    {
        #region Properties and fields

        private IBlogCategoryService _blogCategoryService { get; }
        private ILogService _logService { get; }

        private List<BlogCategory> _allEntities { get; set; }
        private BlogCategoryModel _model;
        private BlogCategory _entity;

        #endregion


        #region Constructor

        public BlogCategoryController(IBlogCategoryService blogCategoryService, ILogService logService)
        {
            _blogCategoryService = blogCategoryService;
            _logService = logService;

            _allEntities = new List<BlogCategory>();
            _model = new BlogCategoryModel();
            _entity = new BlogCategory();
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
            List<BlogCategoryModel> convertedEntityList = new List<BlogCategoryModel>();

            _allEntities = await _blogCategoryService.ListAsync();

            List<BlogCategory> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new BlogCategoryModel();
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
            _entity = await _blogCategoryService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            EmptyModel(ref _model);
            PrepareDropdown(ref _model, await _blogCategoryService.ListAsync());

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(BlogCategoryModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);
                await _blogCategoryService.CreateAsync(_entity);

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
                _entity = await _blogCategoryService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            EntityToModel(_entity, ref _model);

            PrepareDropdown(ref _model, await _blogCategoryService.ListAsync());

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(BlogCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _blogCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);
                await _blogCategoryService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "BlogCategory");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<BlogCategoryModel> convertedEntityList = new List<BlogCategoryModel>();
            List<BlogCategory> removedEntityList = await _blogCategoryService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new BlogCategoryModel();
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
                await _blogCategoryService.DeleteAsync(id);

                await Log(LogType.ContentDelete, "Delete", $"{_entity.Id}_ {_entity.Title}");
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
                await _blogCategoryService.RemoveAsync(id);
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
                await _blogCategoryService.RestoreAsync(id);
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
                await _blogCategoryService.DeleteRangeAsync(await _blogCategoryService.ListOfRemovedAsync());
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

        private void EntityToModel(BlogCategory entity, ref BlogCategoryModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Priority = entity.Priority;
            model.ParentId = entity.ParentId;
            model.ParentTitle = entity.Parent?.Title ?? " - ";
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(BlogCategory entity, ref BlogCategoryModel model, int index)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.index = index;
            model.Title = entity.Title;
            model.Priority = entity.Priority;
            model.ParentId = entity.ParentId;
            model.ParentTitle = entity.Parent?.Title ?? " - ";
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void ModelToEntity(BlogCategoryModel model, ref BlogCategory entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.Priority = model.Priority ?? 0;
            entity.ParentId = model.ParentId;
        }

        private void EmptyEntity(ref BlogCategory entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.Priority = 0;
            entity.ParentId = 0;
        }

        private void EmptyModel(ref BlogCategoryModel model)
        {
            model.Id = 0;
            model.index = 0;
            model.Title = "";
            model.Priority = 0;
            model.ParentId = 0;
            model.ParentTitle = "";
            model.ParentDropdown.Clear();
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

        private void PrepareDropdown(ref BlogCategoryModel model, List<BlogCategory> dropdownItemsList)
        {
            model.ParentDropdown.Clear();

            model.ParentDropdown.Add(new SelectListItem()
            {
                Text = "Select an item...",
                Value = "",
            });

            int  modelId = model.Id;
            dropdownItemsList.Remove(dropdownItemsList.FirstOrDefault(x => x.Id == modelId));

            foreach (var entity in dropdownItemsList)
            {
                model.ParentDropdown.Add(new SelectListItem()
                {
                    Text = entity.Title,
                    Value = entity.Id.ToString(),
                });
            }
        }
        
        #endregion
    }
}