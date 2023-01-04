using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class DiscountCategoryController : Controller
    {
        #region Properties and fields

        private IDiscountCategoryService _discountCategoryService { get; }
        private IProductService _productService { get; }
        private ILogService _logService { get; }

        private List<DiscountCategory> _allEntities { get; set; }
        private DiscountCategoryModel _model;
        private DiscountCategory _entity;

        #endregion


        #region Constructor

        public DiscountCategoryController(IDiscountCategoryService discountCategoryService, ILogService logService, IProductService productService)
        {
            _discountCategoryService = discountCategoryService;
            _logService = logService;
            _productService = productService;

            _allEntities = new List<DiscountCategory>();
            _model = new DiscountCategoryModel();
            _entity = new DiscountCategory();
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
            List<DiscountCategoryModel> convertedEntityList = new List<DiscountCategoryModel>();

            _allEntities = await _discountCategoryService.ListAsync();

            List<DiscountCategory> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new DiscountCategoryModel();
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
            _entity = await _discountCategoryService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            EmptyModel(ref _model);
            PrepareDropdown(ref _model, await _productService.ListAsync());

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(DiscountCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.CreateDateTime = DateTime.UtcNow;
                _entity.UpdateDateTime = DateTime.UtcNow;

                await _discountCategoryService.CreateAsync(_entity);

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
                _entity = await _discountCategoryService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            EntityToModel(_entity, ref _model);

            PrepareDropdown(ref _model, await _productService.ListAsync());

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(DiscountCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.UpdateDateTime = DateTime.UtcNow;

                await _discountCategoryService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "DiscountCategory");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<DiscountCategoryModel> convertedEntityList = new List<DiscountCategoryModel>();
            List<DiscountCategory> removedEntityList = await _discountCategoryService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new DiscountCategoryModel();
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
                await _discountCategoryService.DeleteAsync(id);

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
                await _discountCategoryService.RemoveAsync(id);
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
                await _discountCategoryService.RestoreAsync(id);
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
                await _discountCategoryService.DeleteRangeAsync(await _discountCategoryService.ListOfRemovedAsync());
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

        private void EntityToModel(DiscountCategory entity, ref DiscountCategoryModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Discount = entity.Discount;
            model.IsActive= entity.IsActive;
            model.ProductIds = entity.Products?.Select(c => c.Id).ToList();
            model.UpdateDateTime = entity.UpdateDateTime;
            model.ExpiryDateTime = entity.ExpiryDateTime;
        }

        private void EntityToModel(DiscountCategory entity, ref DiscountCategoryModel model, int index)
        {
            EmptyModel(ref model);

            model.Index = index;
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Discount = entity.Discount;
            model.IsActive = entity.IsActive;
            model.ProductIds = entity.Products?.Select(c => c.Id).ToList();
            model.UpdateDateTime = entity.UpdateDateTime;
            model.ExpiryDateTime = entity.ExpiryDateTime;
        }

        private void ModelToEntity(DiscountCategoryModel model, ref DiscountCategory entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.Discount = model.Discount;
            entity.IsActive = model.IsActive;
            entity.UpdateDateTime = model.UpdateDateTime;
            entity.ExpiryDateTime = model.ExpiryDateTime;

            foreach (var id in model.ProductIds)
            {
                var category = _productService.FindByIdAsync(id);
                category.Wait();

                entity.Products.Add(category.Result);
            }
        }

        private void EmptyEntity(ref DiscountCategory entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.Discount = 0;
            entity.IsActive = false;
            entity.Products.Clear();
            entity.UpdateDateTime = DateTime.UtcNow;
            entity.ExpiryDateTime = DateTime.UtcNow;

        }

        private void EmptyModel(ref DiscountCategoryModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.Title = "";
            model.Discount = 0;
            model.IsActive = false;
            model.ProductIds.Clear();
            model.ProductDropdown.Clear();
            model.UpdateDateTime = DateTime.UtcNow;
            model.ExpiryDateTime = DateTime.UtcNow;
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

        private void PrepareDropdown(ref DiscountCategoryModel model, List<Product> dropdownItemsList)
        {
            model.ProductDropdown.Clear();

            foreach (var entity in dropdownItemsList)
            {
                model.ProductDropdown.Add(new SelectListItem()
                {
                    Text = entity.Title,
                    Value = entity.Id.ToString(),
                });
            }
        }
        
        #endregion
    }
}