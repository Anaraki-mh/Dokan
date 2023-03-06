using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using Dokan.Web.Helpers;
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
    public class BlogCategoryController : ManagementBaseController
    {
        #region Properties and fields

        private IBlogCategoryService _blogCategoryService { get; }
        private ILogService _logService { get; }

        #endregion


        #region Constructor

        public BlogCategoryController(IBlogCategoryService blogCategoryService, ILogService logService)
        {
            _blogCategoryService = blogCategoryService;
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
            List<BlogCategoryModel> convertedEntityList = new List<BlogCategoryModel>();

            var allEntities = await _blogCategoryService.ListAsync();

            List<BlogCategory> filteredList = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                var model = BlogCategoryModel.EntityToModel(in entity, index);

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
            var entity = await _blogCategoryService.FindByIdAsync(id);

            var model = BlogCategoryModel.EntityToModel(in entity);

            return PartialView("_Details", model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new BlogCategoryModel();
            BlogCategoryModel.PrepareDropdown(ref model, await _blogCategoryService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(BlogCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                BlogCategoryModel.PrepareDropdown(ref model, await _blogCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                var entity = BlogCategoryModel.ModelToEntity(in model);

                entity.CreateDateTime = DateTime.UtcNow;
                entity.UpdateDateTime = DateTime.UtcNow;

                await _blogCategoryService.CreateAsync(entity);

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
            var entity = new BlogCategory();

            try
            {
                entity = await _blogCategoryService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            var model = BlogCategoryModel.EntityToModel(in entity);

            BlogCategoryModel.PrepareDropdown(ref model, await _blogCategoryService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(BlogCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                BlogCategoryModel.PrepareDropdown(ref model, await _blogCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                var entity = BlogCategoryModel.ModelToEntity(in model);
                entity.UpdateDateTime = DateTime.UtcNow;

                await _blogCategoryService.UpdateAsync(entity);

                await Log(LogType.ContentUpdate, "Update", $"{entity.Id}_ {entity.Title}");
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
                var model = BlogCategoryModel.EntityToModel(in entity, index);
                convertedEntityList.Add(model);
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