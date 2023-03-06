using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using Dokan.WebEssentials;
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
    public class ProductCommentController : ManagementBaseController
    {
        #region Properties and fields

        private IProductCommentService _productCommentService { get; }
        private ILogService _logService { get; }
        private IEmailService _emailService { get; }

        #endregion


        #region Constructor

        public ProductCommentController(IProductCommentService ProductCommentService, ILogService logService, IEmailService emailService)
        {
            _productCommentService = ProductCommentService;
            _logService = logService;
            _emailService = emailService;
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
            List<ProductCommentModel> convertedEntityList = new List<ProductCommentModel>();

            var allEntities = await _productCommentService.ListAsync();

            List<ProductComment> filteredList = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                var model = ProductCommentModel.EntityToModel(in entity, index);

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
            var entity = await _productCommentService.FindByIdAsync(id);
            var model = ProductCommentModel.EntityToModel(in entity);
            return PartialView("_Details", model);
        }

        [HttpGet]
        public async Task<ActionResult> Reply(int id)
        {
            var entity = await _productCommentService.FindByIdAsync(id);
            var model = new ProductCommentModel();

            model.ProductId = entity.Id;
            model.ParentId = entity.Id;
            model.UserId = User.Identity.GetUserId();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Reply(ProductCommentModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.ParentId == 0)
                return View("Error400");

            try
            {
                var entity = ProductCommentModel.ModelToEntity(in model);

                entity.IsApproved = true;

                await _productCommentService.CreateAsync(entity);

                var comment = await _productCommentService.FindByIdAsync(model.ParentId);
                var recipientEmail = comment.User.Email;

                string emailBody = EmailTemplate.PrepareReply("An admin has replied on your comment!",
                    "",
                    model.Body,
                    EmailTemplate.CreateButton("Open Website", EmailTemplate.WebAddress));

                _emailService.SendEmail("Someone has replied on your comment!", emailBody, recipientEmail);

                await Log(LogType.ContentAdd, "Reply", $"{entity.Id}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "ProductComment");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<ProductCommentModel> convertedEntityList = new List<ProductCommentModel>();
            List<ProductComment> removedEntityList = await _productCommentService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                var model = ProductCommentModel.EntityToModel(in entity, index);
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
                await _productCommentService.DeleteAsync(id);

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
                await _productCommentService.RemoveAsync(id);
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
                await _productCommentService.RestoreAsync(id);
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
                await _productCommentService.DeleteRangeAsync(await _productCommentService.ListOfRemovedAsync());
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
                var entity = await _productCommentService.FindByIdAsync(id);
                entity.IsApproved = true;
                await _productCommentService.UpdateAsync(entity);
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
                var entity = await _productCommentService.FindByIdAsync(id);
                entity.IsApproved = false;
                await _productCommentService.UpdateAsync(entity);
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