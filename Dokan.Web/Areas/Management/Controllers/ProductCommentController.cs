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
    public class ProductCommentController : Controller
    {
        #region Properties and fields

        private IProductCommentService _productCommentService { get; }
        private ILogService _logService { get; }

        private List<ProductComment> _allEntities { get; set; }
        private ProductCommentModel _model;
        private ProductComment _entity;

        #endregion


        #region Constructor

        public ProductCommentController(IProductCommentService ProductCommentService, ILogService logService)
        {
            _productCommentService = ProductCommentService;
            _logService = logService;

            _allEntities = new List<ProductComment>();
            _model = new ProductCommentModel();
            _entity = new ProductComment();
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

            _allEntities = await _productCommentService.ListAsync();

            List<ProductComment> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new ProductCommentModel();
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
            _entity = await _productCommentService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public ActionResult Reply(int id)
        {
            EmptyModel(ref _model);
            _model.ProductId = id;
            _model.UserId = User.Identity.GetUserId();

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Reply(ProductCommentModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);

                await _productCommentService.CreateAsync(_entity);

                // use email service (to be made...) to send an email back as the reply

                await Log(LogType.ContentAdd, "Reply", $"{_entity.Id}");
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
                _model = new ProductCommentModel();
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
                _entity = await _productCommentService.FindByIdAsync(id);
                _entity.IsApproved = true;
                await _productCommentService.UpdateAsync(_entity);
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
                _entity = await _productCommentService.FindByIdAsync(id);
                _entity.IsApproved = false;
                await _productCommentService.UpdateAsync(_entity);
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

        private void EntityToModel(ProductComment entity, ref ProductCommentModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.ProductTitle = entity.Product?.Title;
            model.ProductId = entity.ProductId;
            model.Title = entity.Title;
            model.Username = entity.User?.UserName;
            model.UserId = entity.UserId;
            model.Body = entity.Body;
            model.IsApproved = entity.IsApproved;
            model.Rating = entity.Rating;
            model.CreateDateTime = entity.CreateDateTime;
        }

        private void EntityToModel(ProductComment entity, ref ProductCommentModel model, int index)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Index = index;
            model.ProductTitle = entity.Product?.Title;
            model.ProductId = entity.ProductId;
            model.Title = entity.Title;
            model.Username = entity.User?.UserName;
            model.UserId = entity.UserId;
            model.Body = entity.Body;
            model.IsApproved = entity.IsApproved;
            model.Rating = entity.Rating;
            model.CreateDateTime = entity.CreateDateTime;
        }

        private void ModelToEntity(ProductCommentModel model, ref ProductComment entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.ProductId = model.ProductId;
            entity.Title = model.Title;
            entity.UserId = model.UserId;
            entity.Body = model.Body;
            entity.IsApproved = model.IsApproved;
            entity.Rating = model.Rating;
            entity.CreateDateTime = model.CreateDateTime;
        }

        private void EmptyEntity(ref ProductComment entity)
        {
            entity.Id = 0;
            entity.ProductId = 0;
            entity.Title = "";
            entity.UserId = "";
            entity.Body = "";
            entity.IsApproved = false;
            entity.Rating = 0;
            entity.CreateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref ProductCommentModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.ProductTitle = "";
            model.ProductId = 0;
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