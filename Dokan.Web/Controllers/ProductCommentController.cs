﻿using Dokan.Domain.Website;
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

        private ProductCommentModel _model;
        private ProductComment _entity;
        private List<ProductComment> _allEntities { get; set; }

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

            _allEntities = await _productCommentService.ListAsync();

            _allEntities = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in _allEntities)
            {
                _model = new ProductCommentModel();
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
            _model = new ProductCommentModel();
            _model.UserId = User.Identity.GetUserId();

            return PartialView("_Create", _model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductCommentModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create");

            ModelToEntity(model, ref _entity);

            await _productCommentService.CreateAsync(_entity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Delete(int id)
        {
            _entity = await _productCommentService.FindByIdAsync(id);

            if (HttpContext.User.Identity.GetUserId() == _entity.UserId)
                await _productCommentService.DeleteAsync(id);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Reply(int id)
        {
            _entity = await _productCommentService.FindByIdAsync(id);

            if (_entity.Id == id)
            {
                ProductCommentModel model = new ProductCommentModel();
                model.ParentId = id;
                model.ProductId = _entity.ProductId;
                model.UserId = HttpContext.User.Identity.GetUserId();

                return PartialView(model);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        #endregion


        #region Conversion Methods

        private void EntityToModel(ProductComment entity, ref ProductCommentModel model)
        {
            model.Id = entity.Id;
            model.Username = entity.User?.UserName;
            model.UserId = entity.UserId;
            model.UserProfilePic = entity.User?.UserInformation?.ProfilePicture;
            model.ProductId = entity.ProductId;
            model.Body = entity.Body;
            model.Rating = entity.Rating;
            model.CreateDateTime = $"{entity.CreateDateTime:MMM d yyyy}";
        }

        private void ModelToEntity(ProductCommentModel model, ref ProductComment entity)
        {
            entity.Id = model.Id;
            entity.UserId = model.UserId;
            entity.ProductId = model.ProductId;
            entity.Body = model.Body;
            entity.Rating = model.Rating;
            entity.CreateDateTime = DateTime.UtcNow;
            entity.UpdateDateTime = DateTime.UtcNow;
        }


        #endregion
    }
}