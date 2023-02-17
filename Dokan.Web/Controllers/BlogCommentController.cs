using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dokan.Web.Controllers
{
    [Authorize]
    public class BlogCommentController : Controller
    {
        #region Fields and Properties

        private IBlogCommentService _blogCommentService { get; }
        private IEmailService _emailService { get; }

        private BlogCommentModel _model;
        private BlogComment _entity;
        private List<BlogComment> _allEntities { get; set; }

        #endregion


        #region Contrsuctor

        public BlogCommentController(IBlogCommentService blogCommentService, IEmailService emailService)
        {
            _blogCommentService = blogCommentService;
            _emailService = emailService;
        }

        #endregion


        #region Methods

        [AllowAnonymous]
        public async Task<ActionResult> List(int page = 1)
        {
            int numberOfResults = 10;

            List<BlogCommentModel> convertedEntityList = new List<BlogCommentModel>();

            _allEntities = await _blogCommentService.ListAsync();

            _allEntities = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in _allEntities)
            {
                _model = new BlogCommentModel();
                EntityToModel(entity, ref _model);

                convertedEntityList.Add(_model);

                index++;
            }

            ViewBag.NumberOfPages = Math.Ceiling((decimal)_allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;
            ViewBag.UserId = User.Identity.GetUserId();

            return View("_List", convertedEntityList);
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            _model = new BlogCommentModel();
            _model.UserId = User.Identity.GetUserId();

            return PartialView("_Create", _model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(BlogCommentModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create");

            ModelToEntity(model, ref _entity);

            await _blogCommentService.CreateAsync(_entity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Delete(int id)
        {
            _entity = await _blogCommentService.FindByIdAsync(id);

            if (HttpContext.User.Identity.GetUserId() == _entity.UserId)
                await _blogCommentService.DeleteAsync(id);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Reply(int id)
        {
            _entity = await _blogCommentService.FindByIdAsync(id);

            if (_entity.Id == id)
            {
                BlogCommentModel model = new BlogCommentModel();
                model.ParentId = id;
                model.BlogPostId = _entity.BlogPostId;
                model.UserId = HttpContext.User.Identity.GetUserId();

                return PartialView(model);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        #endregion


        #region Conversion Methods

        private void EntityToModel(BlogComment entity, ref BlogCommentModel model)
        {
            model.Id = entity.Id;
            model.Username = entity.User?.UserName;
            model.UserId = entity.UserId;
            model.UserProfilePic = entity.User?.UserInformation?.ProfilePicture;
            model.BlogPostId = entity.BlogPostId;
            model.Body = entity.Body;
            model.Rating = entity.Rating;
            model.CreateDateTime = $"{entity.CreateDateTime:MMM d yyyy}";
        }

        private void ModelToEntity(BlogCommentModel model, ref BlogComment entity)
        {
            entity.Id = model.Id;
            entity.UserId = model.UserId;
            entity.BlogPostId = model.BlogPostId;
            entity.Body = model.Body;
            entity.Rating = model.Rating;
            entity.CreateDateTime = DateTime.UtcNow;
            entity.UpdateDateTime = DateTime.UtcNow;
        }


        #endregion
    }
}