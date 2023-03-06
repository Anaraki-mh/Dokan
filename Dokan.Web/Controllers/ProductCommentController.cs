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
    public class ProductCommentController : Controller
    {
        #region Fields and Properties

        private IProductCommentService _productCommentService { get; }
        private IEmailService _emailService { get; }

        #endregion


        #region Contrsuctor

        public ProductCommentController(IProductCommentService productCommentService, IEmailService emailService)
        {
            _productCommentService = productCommentService;
            _emailService = emailService;
        }

        #endregion


        #region Methods

        [AllowAnonymous]
        public async Task<ActionResult> List(int page = 1)
        {
            int numberOfResults = 10;

            List<ProductCommentModel> convertedEntityList = new List<ProductCommentModel>();

            var allEntities = await _productCommentService.ListAsync();

            allEntities = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in allEntities)
            {
                var model = ProductCommentModel.EntityToModel(in entity);

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
            var model = new ProductCommentModel();
            model.UserId = User.Identity.GetUserId();

            return PartialView("_Create", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductCommentModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create");

            var entity = ProductCommentModel.ModelToEntity(in model);

            await _productCommentService.CreateAsync(entity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var entity = await _productCommentService.FindByIdAsync(id);

            if (HttpContext.User.Identity.GetUserId() == entity.UserId)
                await _productCommentService.DeleteAsync(id);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Reply(int id)
        {
            var entity = await _productCommentService.FindByIdAsync(id);

            if (entity.Id == id)
            {
                ProductCommentModel model = new ProductCommentModel();
                model.ParentId = id;
                model.ProductId = entity.ProductId;
                model.UserId = HttpContext.User.Identity.GetUserId();

                return PartialView(model);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        #endregion

    }
}