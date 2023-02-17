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
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class TaxCategoryController : ManagementBaseController
    {
        #region Properties and fields

        private ITaxCategoryService _taxCategoryService { get; }
        private IProductCategoryService _productCategoryService { get; }
        private ILogService _logService { get; }

        private List<TaxCategory> _allEntities { get; set; }
        private TaxCategoryModel _model;
        private TaxCategory _entity;

        #endregion


        #region Constructor

        public TaxCategoryController(ITaxCategoryService taxCategoryService, ILogService logService, IProductCategoryService productCategoryService)
        {
            _taxCategoryService = taxCategoryService;
            _logService = logService;
            _productCategoryService = productCategoryService;

            _allEntities = new List<TaxCategory>();
            _model = new TaxCategoryModel();
            _entity = new TaxCategory();
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
            List<TaxCategoryModel> convertedEntityList = new List<TaxCategoryModel>();

            _allEntities = await _taxCategoryService.ListAsync();

            List<TaxCategory> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new TaxCategoryModel();
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
            _entity = await _taxCategoryService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            EmptyModel(ref _model);
            PrepareDropdown(ref _model, await _productCategoryService.ListAsync());

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(TaxCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.CreateDateTime = DateTime.UtcNow;
                _entity.UpdateDateTime = DateTime.UtcNow;

                await _taxCategoryService.CreateAsync(_entity);

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
                _entity = await _taxCategoryService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            EntityToModel(_entity, ref _model);

            PrepareDropdown(ref _model, await _productCategoryService.ListAsync());

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(TaxCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.UpdateDateTime = DateTime.UtcNow;

                await _taxCategoryService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "TaxCategory");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<TaxCategoryModel> convertedEntityList = new List<TaxCategoryModel>();
            List<TaxCategory> removedEntityList = await _taxCategoryService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new TaxCategoryModel();
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
                await _taxCategoryService.DeleteAsync(id);

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
                await _taxCategoryService.RemoveAsync(id);
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
                await _taxCategoryService.RestoreAsync(id);
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
                await _taxCategoryService.DeleteRangeAsync(await _taxCategoryService.ListOfRemovedAsync());
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

        private void EntityToModel(TaxCategory entity, ref TaxCategoryModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Tax = entity.Tax;
            model.CategoryIds = entity.ProductCategories?.Select(c => c.Id).ToList();
            model.CategoryTitles = String.Join(", ", entity.ProductCategories?.Select(c => c.Title).ToList());
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(TaxCategory entity, ref TaxCategoryModel model, int index)
        {
            EmptyModel(ref model);

            model.Index = index;
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Tax = entity.Tax;
            model.CategoryIds = entity.ProductCategories?.Select(c => c.Id).ToList();
            model.CategoryTitles = String.Join(", ", entity.ProductCategories?.Select(c => c.Title).ToList());
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void ModelToEntity(TaxCategoryModel model, ref TaxCategory entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.Tax = model.Tax;
            entity.UpdateDateTime = model.UpdateDateTime;
            foreach (var id in model.CategoryIds)
            {
                var category = _productCategoryService.FindByIdAsync(id);
                category.Wait();

                entity.ProductCategories.Add(category.Result);
            }
        }

        private void EmptyEntity(ref TaxCategory entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.Tax = 0;
            entity.ProductCategories.Clear();
            entity.UpdateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref TaxCategoryModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.Title = "";
            model.Tax = 0;
            model.CategoryTitles = "";
            model.CategoryIds.Clear();
            model.CategoryDropdown.Clear();
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

        private void PrepareDropdown(ref TaxCategoryModel model, List<ProductCategory> dropdownItemsList)
        {
            model.CategoryDropdown.Clear();

            foreach (var entity in dropdownItemsList)
            {
                model.CategoryDropdown.Add(new SelectListItem()
                {
                    Text = entity.Title,
                    Value = entity.Id.ToString(),
                });
            }
        }
        
        #endregion
    }
}