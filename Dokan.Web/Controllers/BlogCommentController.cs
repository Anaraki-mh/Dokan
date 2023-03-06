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

            var allEntities = await _blogCommentService.ListAsync();

            allEntities = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in allEntities)
            {
                var model = BlogCommentModel.EntityToModel(in entity);

                convertedEntityList.Add(model);

                index++;
            }

            ViewBag.NumberOfPages = Math.Ceiling((decimal)allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;
            ViewBag.UserId = User.Identity.GetUserId();

            return View("_List", convertedEntityList);
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            var model = new BlogCommentModel();
            model.UserId = User.Identity.GetUserId();

            return PartialView("_Create", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(BlogCommentModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create");

            var entity = BlogCommentModel.ModelToEntity(in model);

            await _blogCommentService.CreateAsync(entity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var entity = await _blogCommentService.FindByIdAsync(id);

            if (HttpContext.User.Identity.GetUserId() == entity.UserId)
                await _blogCommentService.DeleteAsync(id);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Reply(int id)
        {
            var entity = await _blogCommentService.FindByIdAsync(id);

            if (entity.Id == id)
            {
                BlogCommentModel model = new BlogCommentModel();
                model.ParentId = id;
                model.BlogPostId = entity.BlogPostId;
                model.UserId = HttpContext.User.Identity.GetUserId();

                return PartialView(model);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        #endregion

    }
}