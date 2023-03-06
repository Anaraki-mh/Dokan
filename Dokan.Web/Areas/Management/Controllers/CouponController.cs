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
    public class CouponController : ManagementBaseController
    {
        #region Properties and fields

        private ICouponService _couponService { get; }
        private IProductCategoryService _productCategoryService { get; }
        private ILogService _logService { get; }

        #endregion


        #region Constructor

        public CouponController(ICouponService couponService, ILogService logService, IProductCategoryService productCategoryService)
        {
            _couponService = couponService;
            _logService = logService;
            _productCategoryService = productCategoryService;
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

            var allEntities = await _couponService.ListAsync();

            List<Coupon> filteredList = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                var model = CouponModel.EntityToModel(in entity, index);

                convertedEntityList.Add(model);

                index++;
            }

            ViewBag.NumberOfPages = Math.Ceiling((decimal)allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var entity = await _couponService.FindByIdAsync(id);

            var model = CouponModel.EntityToModel(in entity);

            return PartialView("_Details", model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new CouponModel();
            CouponModel.PrepareDropdown(ref model, await _productCategoryService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CouponModel model)
        {
            if (!ModelState.IsValid)
            {
                CouponModel.PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            var coupon = await _couponService.SearchAsync(model.Code);
            if (coupon.Code != null)
            {
                ViewBag.CodeError = "This code already exists";
                return View(model);
            }

            var entity = new Coupon();
            try
            {
                entity  = CouponModel.ModelToEntity(in model, await _productCategoryService.ListAsync());

                entity.CreateDateTime = DateTime.UtcNow;
                entity.UpdateDateTime = DateTime.UtcNow;

                if(entity.ExpiryDateTime < entity.UpdateDateTime)
                    entity.IsActive= false;

                await _couponService.CreateAsync(entity);

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
            var entity = new Coupon();
            try
            {
                entity = await _couponService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            var model = CouponModel.EntityToModel(in entity);

            CouponModel.PrepareDropdown(ref model, await _productCategoryService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(CouponModel model)
        {
            if (!ModelState.IsValid)
            {
                CouponModel.PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            var entity = new Coupon();
            try
            {
                entity = CouponModel.ModelToEntity(in model, await _productCategoryService.ListAsync());

                entity.UpdateDateTime = DateTime.UtcNow;

                if (entity.ExpiryDateTime < entity.UpdateDateTime)
                    entity.IsActive = false;

                await _couponService.UpdateAsync(entity);

                await Log(LogType.ContentUpdate, "Update", $"{entity.Id}_ {entity.Title}");
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
                var model = CouponModel.EntityToModel(in entity, index);

                convertedEntityList.Add(model);

                index++;
            }


            return PartialView("_TrashList", convertedEntityList);
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