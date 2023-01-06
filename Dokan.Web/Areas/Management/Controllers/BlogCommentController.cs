using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using Microsoft.AspNet.Identity;
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
    public class BlogCommentController : Controller
    {
        #region Properties and fields

        private IBlogCommentService _blogCommentService { get; }
        private ILogService _logService { get; }

        private List<BlogComment> _allEntities { get; set; }
        private BlogCommentModel _model;
        private BlogComment _entity;

        #endregion


        #region Constructor

        public BlogCommentController(IBlogCommentService BlogCommentService, ILogService logService)
        {
            _blogCommentService = BlogCommentService;
            _logService = logService;

            _allEntities = new List<BlogComment>();
            _model = new BlogCommentModel();
            _entity = new BlogComment();
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
            List<BlogCommentModel> convertedEntityList = new List<BlogCommentModel>();

            _allEntities = await _blogCommentService.ListAsync();

            List<BlogComment> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new BlogCommentModel();
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
            _entity = await _blogCommentService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public async Task<ActionResult> Reply(int id)
        {
            _entity = await _blogCommentService.FindByIdAsync(id);

            EmptyModel(ref _model);
            _model.BlogPostId = _entity.Id;
            _model.ParentId = _entity.Id;
            _model.UserId = User.Identity.GetUserId();


            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Reply(BlogCommentModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if(model.ParentId == 0)
                return View("Error400");

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.IsApproved = true;

                await _blogCommentService.CreateAsync(_entity);

                // use email service (to be made...) to send an email back as the reply

                await Log(LogType.ContentAdd, "Reply", $"{_entity.Id}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "BlogComment");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<BlogCommentModel> convertedEntityList = new List<BlogCommentModel>();
            List<BlogComment> removedEntityList = await _blogCommentService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new BlogCommentModel();
                EntityToModel(entity, ref _model, index);

                convertedEntityList.Add(_model);

                index++;
            }


            return PartialView("_TrashList", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _blogCommentService.DeleteAsync(id);

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
                await _blogCommentService.RemoveAsync(id);
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
                await _blogCommentService.RestoreAsync(id);
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
                await _blogCommentService.DeleteRangeAsync(await _blogCommentService.ListOfRemovedAsync());
                await Log(LogType.ContentAdd, "DeleteAllTrash", $"Deleted all items in the trash");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> Approve(int id)
        {
            try
            {
                _entity = await _blogCommentService.FindByIdAsync(id);
                _entity.IsApproved = true;
                await _blogCommentService.UpdateAsync(_entity);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> Disapprove(int id)
        {
            try
            {
                _entity = await _blogCommentService.FindByIdAsync(id);
                _entity.IsApproved = false;
                await _blogCommentService.UpdateAsync(_entity);
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

        private void EntityToModel(BlogComment entity, ref BlogCommentModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.BlogTitle = entity.BlogPost?.Title;
            model.BlogPostId = entity.BlogPostId;
            model.Title = entity.Title;
            model.Username = entity.User?.UserName;
            model.UserId = entity.UserId;
            model.Body = entity.Body;
            model.IsApproved = entity.IsApproved;
            model.Rating = entity.Rating;
            model.CreateDateTime = entity.CreateDateTime;
        }

        private void EntityToModel(BlogComment entity, ref BlogCommentModel model, int index)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Index = index;
            model.BlogTitle = entity.BlogPost?.Title;
            model.BlogPostId = entity.BlogPostId;
            model.Title = entity.Title;
            model.Username = entity.User?.UserName;
            model.UserId = entity.UserId;
            model.Body = entity.Body;
            model.IsApproved = entity.IsApproved;
            model.Rating = entity.Rating;
            model.CreateDateTime = entity.CreateDateTime;
        }

        private void ModelToEntity(BlogCommentModel model, ref BlogComment entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.BlogPostId = model.BlogPostId;
            entity.Title = model.Title;
            entity.UserId = model.UserId;
            entity.Body = model.Body;
            entity.IsApproved = model.IsApproved;
            entity.Rating = model.Rating;
            entity.CreateDateTime = model.CreateDateTime;
        }

        private void EmptyEntity(ref BlogComment entity)
        {
            entity.Id = 0;
            entity.BlogPostId = 0;
            entity.Title = "";
            entity.UserId = "";
            entity.Body = "";
            entity.IsApproved = false;
            entity.Rating = 0;
            entity.CreateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref BlogCommentModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.BlogTitle = "";
            model.BlogPostId = 0;
            model.Title = "";
            model.Username = "";
            model.UserId = "";
            model.Body = "";
            model.IsApproved = false;
            model.Rating = 0;
            model.CreateDateTime = DateTime.UtcNow;
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

        #endregion
    }
}