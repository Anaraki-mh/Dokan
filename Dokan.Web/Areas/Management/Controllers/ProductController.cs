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
    public class ProductController : Controller
    {
        #region Properties and fields

        private IProductService _productService { get; }
        private IProductCategoryService _productCategoryService { get; }
        private ILogService _logService { get; }

        private List<Product> _allEntities { get; set; }
        private ProductModel _model;
        private Product _entity;

        #endregion


        #region Constructor

        public ProductController(IProductService productService, ILogService logService, IProductCategoryService productCategoryService)
        {
            _productService = productService;
            _logService = logService;
            _productCategoryService = productCategoryService;

            _allEntities = new List<Product>();
            _model = new ProductModel();
            _entity = new Product();
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
            List<ProductModel> convertedEntityList = new List<ProductModel>();

            _allEntities = await _productService.ListAsync();

            List<Product> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new ProductModel();
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
            _entity = await _productService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            EmptyModel(ref _model);
            PrepareDropdown(ref _model, await _productCategoryService.ListAsync());

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);
                await _productService.CreateAsync(_entity);

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
                _entity = await _productService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            EntityToModel(_entity, ref _model);

            PrepareDropdown(ref _model, await _productCategoryService.ListAsync());

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(ProductModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareDropdown(ref model, await _productCategoryService.ListAsync());
                return View(model);
            }

            try
            {
                ModelToEntity(model, ref _entity);
                await _productService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<ProductModel> convertedEntityList = new List<ProductModel>();
            List<Product> removedEntityList = await _productService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new ProductModel();
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
                await _productService.DeleteAsync(id);

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
                await _productService.RemoveAsync(id);
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
                await _productService.RestoreAsync(id);
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
                await _productService.DeleteRangeAsync(await _productService.ListOfRemovedAsync());
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

        private void EntityToModel(Product entity, ref ProductModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.ShortDescription = entity.ShortDescription;
            model.Description = entity.Description;
            model.Price= entity.Price;
            model.Stock= entity.Stock;
            model.Image1 = entity.Image1;
            model.Image2 = entity.Image2;
            model.Image3 = entity.Image3;
            model.Image4 = entity.Image4;
            model.Image5 = entity.Image5;
            model.CategoryId = entity.ProductCategoryId;
            model.CategoryTitle = entity.ProductCategory?.Title ?? " - ";
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(Product entity, ref ProductModel model, int index)
        {
            EmptyModel(ref model);

            model.Index = index;
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.ShortDescription = entity.ShortDescription;
            model.Description = entity.Description;
            model.Price = entity.Price;
            model.Stock = entity.Stock;
            model.Image1 = entity.Image1;
            model.Image2 = entity.Image2;
            model.Image3 = entity.Image3;
            model.Image4 = entity.Image4;
            model.Image5 = entity.Image5;
            model.CategoryId = entity.ProductCategoryId;
            model.CategoryTitle = entity.ProductCategory?.Title ?? " - ";
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void ModelToEntity(ProductModel model, ref Product entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.ShortDescription = model.ShortDescription;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.Stock = model.Stock;
            entity.Image1 = model.Image1;
            entity.Image2 = model.Image2;
            entity.Image3 = model.Image3;
            entity.Image4 = model.Image4;
            entity.Image5 = model.Image5;
            entity.ProductCategoryId = model.CategoryId;
            entity.UpdateDateTime = model.UpdateDateTime;
        }

        private void EmptyEntity(ref Product entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.ShortDescription = "";
            entity.Description = "";
            entity.Price = 0;
            entity.Stock = 0;
            entity.Image1 = "";
            entity.Image2 = "";
            entity.Image3 = "";
            entity.Image4 = "";
            entity.Image5 = "";
            entity.ProductCategoryId = 0;
            entity.UpdateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref ProductModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.Title = "";
            model.ShortDescription = "";
            model.Description = "";
            model.Price = 0;
            model.Stock = 0;
            model.Image1 = "";
            model.Image2 = "";
            model.Image3 = "";
            model.Image4 = "";
            model.Image5 = "";
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

        private void PrepareDropdown(ref ProductModel model, List<ProductCategory> dropdownItemsList)
        {
            model.CategoryDropdown.Clear();

            model.CategoryDropdown.Add(new SelectListItem()
            {
                Text = "Select an item...",
                Value = "",
            });

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