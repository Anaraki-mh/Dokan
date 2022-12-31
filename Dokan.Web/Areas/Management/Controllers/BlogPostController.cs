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
    public class BlogPostController : Controller
    {
        #region Properties and fields

        private IBlogPostService _blogPostService { get; }
        private IBlogCategoryService _blogCategoryService { get; }
        private ILogService _logService { get; }

        private List<BlogPost> _allEntities { get; set; }
        private BlogPostModel _model;
        private BlogPost _entity;

        #endregion


        #region Constructor

        public BlogPostController(IBlogPostService blogPostService, ILogService logService, IBlogCategoryService blogCategoryService)
        {
            _blogPostService = blogPostService;
            _logService = logService;
            _blogCategoryService = blogCategoryService;

            _allEntities = new List<BlogPost>();
            _model = new BlogPostModel();
            _entity = new BlogPost();
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

            _allEntities = await _blogPostService.ListAsync();

            List<BlogPost> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new BlogPostModel();
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
            _entity = await _blogPostService.FindByIdAsync(id);

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
        public async Task<ActionResult> Create(BlogPostModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);
                await _blogPostService.CreateAsync(_entity);

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
                _entity = await _blogPostService.FindByIdAsync(id);
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
        public async Task<ActionResult> Update(BlogPostModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _blogCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);
                await _blogPostService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
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
                _model = new BlogPostModel();
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


        #region Conversion Methods

        private void EntityToModel(BlogPost entity, ref BlogPostModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.ShortDescription = entity.ShortDescription;
            model.Content = entity.Content;
            model.Image = entity.Image;
            model.CategoryId = entity.BlogCategoryId;
            model.CategoryTitle = entity.BlogCategory?.Title ?? " - ";
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(BlogPost entity, ref BlogPostModel model, int index)
        {
            EmptyModel(ref model);

            model.Index = index;
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.ShortDescription = entity.ShortDescription;
            model.Content = entity.Content;
            model.Image = entity.Image;
            model.CategoryId = entity.BlogCategoryId;
            model.CategoryTitle = entity.BlogCategory?.Title ?? " - ";
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void ModelToEntity(BlogPostModel model, ref BlogPost entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.ShortDescription = model.ShortDescription;
            entity.Content = model.Content;
            entity.Image = model.Image;
            entity.BlogCategoryId = model.CategoryId;
            entity.UpdateDateTime = model.UpdateDateTime;
        }

        private void EmptyEntity(ref BlogPost entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.ShortDescription = "";
            entity.Content = "";
            entity.Image = "";
            entity.BlogCategoryId = 0;
            entity.UpdateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref BlogPostModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.Title = "";
            model.ShortDescription = "";
            model.Content = "";
            model.Image = "";
            model.CategoryId = 0;
            model.CategoryTitle = "";
            model.UpdateDateTime = DateTime.UtcNow;
            model.CategoryDropdown.Clear();
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

        private void PrepareDropdown(ref BlogPostModel model, List<BlogCategory> dropdownItemsList)
        {
            model.CategoryDropdown.Clear();

            model.CategoryDropdown.Add(new SelectListItem()
            {
                Text = "Select an item...",
                Value = "",
            });

            int  modelId = model.Id;
            dropdownItemsList.Remove(dropdownItemsList.FirstOrDefault(x => x.Id == modelId));

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