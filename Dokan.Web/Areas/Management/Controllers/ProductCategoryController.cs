using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class ProductCategoryController : Controller
    {
        #region Properties and fields

        private IProductCategoryService _productCategoryService { get; }
        private ILogService _logService { get; }

        private List<ProductCategory> _allEntities { get; set; }
        private ProductCategoryModel _model;
        private ProductCategory _entity;

        #endregion


        #region Constructor

        public ProductCategoryController(IProductCategoryService ProductCategoryService, ILogService logService)
        {
            _productCategoryService = ProductCategoryService;
            _logService = logService;

            _allEntities = new List<ProductCategory>();
            _model = new ProductCategoryModel();
            _entity = new ProductCategory();
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
            List<ProductCategoryModel> convertedEntityList = new List<ProductCategoryModel>();

            _allEntities = await _productCategoryService.ListAsync();

            List<ProductCategory> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new ProductCategoryModel();
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
            _entity = await _productCategoryService.FindByIdAsync(id);

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
        public async Task<ActionResult> Create(ProductCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);
                await _productCategoryService.CreateAsync(_entity);

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
                _entity = await _productCategoryService.FindByIdAsync(id);
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
        public async Task<ActionResult> Update(ProductCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);
                await _productCategoryService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "ProductCategory");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<ProductCategoryModel> convertedEntityList = new List<ProductCategoryModel>();
            List<ProductCategory> removedEntityList = await _productCategoryService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new ProductCategoryModel();
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
                await _productCategoryService.DeleteAsync(id);

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
                await _productCategoryService.RemoveAsync(id);
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
                await _productCategoryService.RestoreAsync(id);
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
                await _productCategoryService.DeleteRangeAsync(await _productCategoryService.ListOfRemovedAsync());
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

        private void EntityToModel(ProductCategory entity, ref ProductCategoryModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Priority = entity.Priority;
            model.ParentId = entity.ParentId;
            model.ParentTitle = entity.Parent?.Title ?? " - ";
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(ProductCategory entity, ref ProductCategoryModel model, int index)
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

        private void ModelToEntity(ProductCategoryModel model, ref ProductCategory entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.Priority = model.Priority ?? 0;
            entity.ParentId = model.ParentId;
        }

        private void EmptyEntity(ref ProductCategory entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.Priority = 0;
            entity.ParentId = 0;
        }

        private void EmptyModel(ref ProductCategoryModel model)
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
                Description = description,
            });
        }

        private void PrepareDropdown(ref ProductCategoryModel model, List<ProductCategory> dropdownItemsList)
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