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
    public class BlogController : Controller
    {

        #region Fields and Properties

        private IBlogPostService _blogPostService { get; }
        private IBlogCategoryService _blogCategoryService { get; }

        #endregion


        #region Constructor

        public BlogController(IBlogPostService blogService, IBlogCategoryService blogCategoryService)
        {
            _blogPostService = blogService;
            _blogCategoryService = blogCategoryService;

            LayoutHelper.PrepareLayout();
        }

        #endregion


        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            // put in view
            ViewBag.Active = "Blog";
            ViewBag.Title = "Blog";

            return View(0);
        }

        public async Task<ActionResult> Category(int id)
        {
            ViewBag.Active = "Shop";
            ViewBag.Title = "Products";
            ViewBag.OnlyDeals = false;

            BlogCategory category = await _blogCategoryService.FindByIdAsync(id);

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
        public async Task<ActionResult> List(int blogCategoryId, int page = 1)
        {
            // Number of displayed results on each page
            int numberOfResults = 12;

            // A list of all the filtered and converted entities that gets returned with the view
            List<BlogPostModel> convertedEntityList = new List<BlogPostModel>();

            // A list of all the blogs
            var allEntities = await _blogPostService.ListAsync();

            // If a category is selected...
            if (blogCategoryId > 0)
                // Only keep the entities with that category in the list
                allEntities = allEntities.Where(x => x.BlogCategoryId == blogCategoryId).ToList();

            // Based on the numberOfResults and the page number, skip an appropriate number of entities and keep 
            // as many as numberOfResults (pagination)
            allEntities = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            // Convert all the filtered entities into BlogPostModels and add them to convertedEntityList.
            foreach (var entity in allEntities)
            {
                var model = BlogPostModel.EntityToModel(in entity);
                convertedEntityList.Add(model);
            }

            // Pass the number of pages and the active page to the view to display
            ViewBag.NumberOfPages = Math.Ceiling((decimal)allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Filters()
        {
            List<BlogCategory> entities = await _blogCategoryService.ListAsync();

            return PartialView("_Filters", entities);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            BlogPost entity = await _blogPostService.FindByIdAsync(id);

            if (entity.Id != id)
                return RedirectToAction("Index");

            var model = BlogPostModel.EntityToModel(in entity);

            ViewBag.Title = entity.Title;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Search(string searchString)
        {
            var allEntities = await _blogPostService.SearchAsync(searchString);

            List<BlogPostModel> searchResults = new List<BlogPostModel>();


            foreach (var entity in allEntities)
            {
                var model = BlogPostModel.EntityToModel(in entity);
                searchResults.Add(model);
            }

            return PartialView("_List", searchResults);
        }

        #endregion

    }
}