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
    public class BlogPostController : ManagementBaseController
    {
        #region Properties and fields

        private IBlogPostService _blogPostService { get; }
        private IBlogCategoryService _blogCategoryService { get; }
        private ILogService _logService { get; }

        #endregion


        #region Constructor

        public BlogPostController(IBlogPostService blogPostService, ILogService logService, IBlogCategoryService blogCategoryService)
        {
            _blogPostService = blogPostService;
            _logService = logService;
            _blogCategoryService = blogCategoryService;
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
            List<BlogPostModel> convertedEntityList = new List<BlogPostModel>();

            var allEntities = await _blogPostService.ListAsync();

            List<BlogPost> filteredList = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                var model = BlogPostModel.EntityToModel(in entity, index);

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
            var entity = await _blogPostService.FindByIdAsync(id);

            var model = BlogPostModel.EntityToModel(in entity);

            return PartialView("_Details", model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new BlogPostModel();
            BlogPostModel.PrepareDropdown(ref model, await _blogCategoryService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(BlogPostModel model)
        {
            if (!ModelState.IsValid)
            {
                BlogPostModel.PrepareDropdown(ref model, await _blogCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                var entity = BlogPostModel.ModelToEntity(in model);

                entity.CreateDateTime = DateTime.UtcNow;
                entity.UpdateDateTime = DateTime.UtcNow;

                await _blogPostService.CreateAsync(entity);

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
            var entity = new BlogPost();
            try
            {
                entity = await _blogPostService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            var model = BlogPostModel.EntityToModel(in entity);

            BlogPostModel.PrepareDropdown(ref model, await _blogCategoryService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(BlogPostModel model)
        {
            if (!ModelState.IsValid)
            {
                BlogPostModel.PrepareDropdown(ref model, await _blogCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                var entity = BlogPostModel.ModelToEntity(in model);

                entity.UpdateDateTime = DateTime.UtcNow;

                await _blogPostService.UpdateAsync(entity);

                await Log(LogType.ContentUpdate, "Update", $"{entity.Id}_ {entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "BlogPost");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<BlogPostModel> convertedEntityList = new List<BlogPostModel>();
            List<BlogPost> removedEntityList = await _blogPostService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                var model = BlogPostModel.EntityToModel(entity, index);

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
                await _blogPostService.DeleteAsync(id);

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
                await _blogPostService.RemoveAsync(id);
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
                await _blogPostService.RestoreAsync(id);
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
                await _blogPostService.DeleteRangeAsync(await _blogPostService.ListOfRemovedAsync());
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