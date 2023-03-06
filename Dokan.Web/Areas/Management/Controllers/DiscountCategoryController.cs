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
using System.Web.Mvc;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class DiscountCategoryController : ManagementBaseController
    {
        #region Properties and fields

        private IDiscountCategoryService _discountCategoryService { get; }
        private IProductService _productService { get; }
        private IProductCategoryService _productCategoryService { get; }
        private ILogService _logService { get; }

        #endregion


        #region Constructor

        public DiscountCategoryController(IDiscountCategoryService discountCategoryService, ILogService logService, IProductService productService, IProductCategoryService productCategoryService)
        {
            _discountCategoryService = discountCategoryService;
            _logService = logService;
            _productService = productService;
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
            List<DiscountCategoryModel> convertedEntityList = new List<DiscountCategoryModel>();

            var allEntities = await _discountCategoryService.ListAsync();

            List<DiscountCategory> filteredList = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                var model = DiscountCategoryModel.EntityToModel(in entity, index);

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
            var entity = await _discountCategoryService.FindByIdAsync(id);

            var model = DiscountCategoryModel.EntityToModel(in entity);

            return PartialView("_Details", model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new DiscountCategoryModel();
            DiscountCategoryModel.PrepareDropdown(ref model, await _productService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(DiscountCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                DiscountCategoryModel.PrepareDropdown(ref model, await _productService.ListAsync());
                return View(model);
            }

            var entity = new DiscountCategory();
            try
            {
                entity  = DiscountCategoryModel.ModelToEntity(in model, await _productService.ListAsync());

                entity.CreateDateTime = DateTime.UtcNow;
                entity.UpdateDateTime = DateTime.UtcNow;

                await _discountCategoryService.CreateAsync(entity);

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
            var entity = new DiscountCategory();
            try
            {
                entity = await _discountCategoryService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            var model = DiscountCategoryModel.EntityToModel(in entity);

            DiscountCategoryModel.PrepareDropdown(ref model, await _productService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(DiscountCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                DiscountCategoryModel.PrepareDropdown(ref model, await _productService.ListAsync());
                return View(model);
            }

            var entity = new DiscountCategory();
            try
            {
                entity = DiscountCategoryModel.ModelToEntity(in model, await _productService.ListAsync());

                entity.UpdateDateTime = DateTime.UtcNow;

                await _discountCategoryService.UpdateAsync(entity);

                await Log(LogType.ContentUpdate, "Update", $"{entity.Id}_ {entity.Title}");
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
                var model = DiscountCategoryModel.EntityToModel(in entity, index);
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