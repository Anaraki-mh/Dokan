using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Helpers;
using Dokan.Web.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dokan.Web.Controllers
{
    public class ProductsController : Controller
    {

        #region Fields and Properties

        private IProductService _productService { get; }
        private IProductCategoryService _productCategoryService { get; }

        private ProductModel _model;
        private Product _entity;
        private List<Product> _allEntities { get; set; }

        #endregion


        #region Constructor

        public ProductsController(IProductService productService, IProductCategoryService productCategoryService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;

            _allEntities = new List<Product>();
            _model = new ProductModel();
            _entity = new Product();

            LayoutHelper.PrepareLayout();
        }

        #endregion


        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Active = "Shop";
            ViewBag.Title = "Products";
            ViewBag.OnlyDeals = "false";

            return View(0);
        }

        [HttpGet]
        public async Task<ActionResult> Category(int id)
        {
            ViewBag.Active = "Shop";
            ViewBag.Title = "Products";
            ViewBag.OnlyDeals = "false";

            ProductCategory category = await _productCategoryService.FindByIdAsync(id);

            if (category != null && category?.Id != 0)
            {
                ViewBag.Title = $"Products - {category.Title}";
                return View("Index", id);
            }

            // return the exact same view as Index with an int of product category 
            // Js checks if it has a value except 0 and null to send the ajax request only for that category,
            return View("Index", 0);
        }

        [HttpGet]
        public ActionResult Deals()
        {
            ViewBag.Title = "Deals";
            ViewBag.OnlyDeals = "true";

            return View("Index");
        }

        [HttpGet]
        public async Task<ActionResult> List(int productCategoryId, int minPrice, int maxPrice, bool onlyDeals, int page = 1)
        {
            // Number of displayed results on each page
            int numberOfResults = 12;

            // A list of all the filtered and converted entities that gets returned with the view
            List<ProductModel> convertedEntityList = new List<ProductModel>();

            // A list of all the products
            _allEntities = await _productService.ListAsync();

            // If onlyDeals is true... (used for the deals page to display only the discounted products)
            if (onlyDeals == true)
                // Only keep the discounted products from the list of all products
                _allEntities = _allEntities.Where(x => x.DiscountCategoryId > 0).ToList();

            // If a category is selected...
            if (productCategoryId > 0)
                // Only keep the entities with that category in the list
                _allEntities = _allEntities.Where(x => x.ProductCategoryId == productCategoryId).ToList();

            // Find the Max and Min prices in the list of products and pass the value to 2 variables
            int absMaxPrice = Convert.ToInt32(Math.Ceiling(_allEntities.Select(x => x.Price).Max()));
            int absMinPrice = Convert.ToInt32(Math.Ceiling(_allEntities.Select(x => x.Price).Min()));

            // If the minPrice is an invalid number, change its value with absMinPrice, otherwise dont change it
            minPrice = minPrice > maxPrice || minPrice < 0 || minPrice < absMinPrice ? absMinPrice : minPrice;

            // If the maxPrice is an invalid number, change its value with absMaxPrice, otherwise dont change it
            maxPrice = maxPrice < minPrice || maxPrice < 0 || maxPrice > absMaxPrice ? absMaxPrice : maxPrice;

            // Filter the list of products based on the Min and Max prices and only keep the ones that are in the range
            _allEntities = _allEntities.Where(x => x.Price >= minPrice && x.Price <= maxPrice).ToList();

            // Based on the numberOfResults and the page number, skip an appropriate number of entities and keep 
            // as many as numberOfResults (pagination)
            _allEntities = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            // Convert all the filtered entities into ProductModels and add them to convertedEntityList.
            foreach (var entity in _allEntities)
            {
                _model = new ProductModel();
                EntityToModel(entity, ref _model);

                convertedEntityList.Add(_model);
            }

            // Pass the number of pages and the active page to the view to display
            ViewBag.NumberOfPages = Math.Ceiling((decimal)_allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Filters()
        {
            List<ProductCategory> entities = await _productCategoryService.ListAsync();
            _allEntities = await _productService.ListAsync();
            entities = entities.OrderBy(x => x.Priority).ToList();

            ViewBag.MinPrice = Convert.ToInt32(Math.Ceiling(_allEntities.Select(x => x.Price).Min()));
            ViewBag.MaxPrice = Convert.ToInt32(Math.Ceiling(_allEntities.Select(x => x.Price).Max()));

            return PartialView("_Filters", entities);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            Product entity = await _productService.FindByIdAsync(id);

            if (entity.Id != id)
                return RedirectToAction("Index");

            EntityToModel(entity, ref _model);

            ViewBag.Title = entity.Title;

            return View(_model);
        }

        [HttpGet]
        public async Task<ActionResult> Search(string searchString)
        {
            _allEntities.Clear();
            _allEntities = await _productService.SearchAsync(searchString);

            List<ProductModel> searchResults = new List<ProductModel>();


            foreach (var entity in _allEntities)
            {
                _model = new ProductModel();
                EntityToModel(entity, ref _model);

                searchResults.Add(_model);
            }

            return PartialView("_List", searchResults);
        }

        #endregion


        #region Conversion methods

        private void EntityToModel(Product entity, ref ProductModel model)
        {
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.ShortDescription = entity.ShortDescription;
            model.Description = entity.Description;

            var tax = entity.ProductCategory.TaxCategory?.Tax ?? 0;
            model.NoDiscountPrice = $"{entity.Price + entity.Price * (double)tax:0.00}";
            model.Price = model.NoDiscountPrice;

            if (entity.DiscountCategory != null)
                model.Price = $"{(entity.Price + entity.Price * (double)entity.ProductCategory.TaxCategory?.Tax - entity.Price * (double)entity.DiscountCategory?.Discount):0.00}";

            model.Stock = entity.Stock;
            model.Images.Clear();
            model.Images.Add(entity.Image1);
            model.Images.Add(entity.Image2);
            model.Images.Add(entity.Image3);
            model.Images.Add(entity.Image4);
            model.Images.Add(entity.Image5);
            model.Rating = 5;

            if (entity.ProductComments.Count > 0)
                model.Rating = (double)entity.ProductComments?.Average(x => x.Rating);

            model.CategoryId = entity.ProductCategoryId;
            model.CategoryTitle = entity.ProductCategory?.Title ?? " - ";
            model.CreateDateTime = entity.CreateDateTime;
        }


        #endregion
    }
}