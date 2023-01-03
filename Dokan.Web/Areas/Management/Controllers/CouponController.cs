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
    public class CouponController : Controller
    {
        #region Properties and fields

        private ICouponService _couponService { get; }
        private IProductCategoryService _productCategoryService { get; }
        private ILogService _logService { get; }

        private List<Coupon> _allEntities { get; set; }
        private CouponModel _model;
        private Coupon _entity;

        #endregion


        #region Constructor

        public CouponController(ICouponService couponService, ILogService logService, IProductCategoryService productCategoryService)
        {
            _couponService = couponService;
            _logService = logService;
            _productCategoryService = productCategoryService;

            _allEntities = new List<Coupon>();
            _model = new CouponModel();
            _entity = new Coupon();
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
            List<CouponModel> convertedEntityList = new List<CouponModel>();

            _allEntities = await _couponService.ListAsync();

            List<Coupon> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new CouponModel();
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
            _entity = await _couponService.FindByIdAsync(id);

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
        public async Task<ActionResult> Create(CouponModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);
                await _couponService.CreateAsync(_entity);

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
                _entity = await _couponService.FindByIdAsync(id);
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
        public async Task<ActionResult> Update(CouponModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);
                await _couponService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "Coupon");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<CouponModel> convertedEntityList = new List<CouponModel>();
            List<Coupon> removedEntityList = await _couponService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new CouponModel();
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
                await _couponService.DeleteAsync(id);

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
                await _couponService.RemoveAsync(id);
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
                await _couponService.RestoreAsync(id);
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
                await _couponService.DeleteRangeAsync(await _couponService.ListOfRemovedAsync());
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

        private void EntityToModel(Coupon entity, ref CouponModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Code = entity.Code;
            model.IsActive = entity.IsActive;
            model.Discount = entity.Discount;
            model.UsageLimit = entity.UsageLimit;
            model.UsageCount = entity.UsageCount;
            model.CategoryIds = entity.ProductCategories?.Select(c => c.Id).ToList();
            model.ExpiryDateTime = entity.ExpiryDateTime;
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(Coupon entity, ref CouponModel model, int index)
        {
            EmptyModel(ref model);

            model.Index = index;
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Code = entity.Code;
            model.IsActive = entity.IsActive;
            model.Discount = entity.Discount;
            model.UsageLimit = entity.UsageLimit;
            model.UsageCount = entity.UsageCount;
            model.CategoryIds = entity.ProductCategories?.Select(c => c.Id).ToList();
            model.ExpiryDateTime = entity.ExpiryDateTime;
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void ModelToEntity(CouponModel model, ref Coupon entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.Code = model.Code;
            entity.IsActive = model.IsActive;
            entity.Discount = model.Discount;
            entity.UsageLimit = model.UsageLimit;
            entity.UsageCount = model.UsageCount;
            entity.ExpiryDateTime = model.ExpiryDateTime;
            entity.UpdateDateTime = model.UpdateDateTime;
            foreach (var id in model.CategoryIds)
            {
                var category = _productCategoryService.FindByIdAsync(id);
                category.Wait();

                entity.ProductCategories.Add(category.Result);
            }
        }

        private void EmptyEntity(ref Coupon entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.Code = "";
            entity.IsActive = false;
            entity.Discount = 0;
            entity.UsageLimit = 0;
            entity.UsageCount = 0;
            entity.ProductCategories.Clear();
            entity.ExpiryDateTime = DateTime.UtcNow;
            entity.UpdateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref CouponModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.Title = "";
            model.Code = "";
            model.IsActive = false;
            model.Discount = 0;
            model.UsageLimit = 0;
            model.UsageCount = 0;
            model.CategoryIds.Clear();
            model.CategoryDropdown.Clear();
            model.ExpiryDateTime = DateTime.UtcNow;
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

        private void PrepareDropdown(ref CouponModel model, List<ProductCategory> dropdownItemsList)
        {
            model.CategoryDropdown.Clear();

            model.CategoryDropdown.Add(new SelectListItem()
            {
                Text = "Select an item...",
                Value = "",
            });

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