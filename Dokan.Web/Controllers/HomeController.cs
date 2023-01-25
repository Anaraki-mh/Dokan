using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Helpers;
using Dokan.Web.Models;
using Dokan.WebEssentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dokan.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Fields and Properties

        private IKeyValueContentService _kvContentService { get; }
        private IMessageService _messageService { get; }
        private ICarouselService _carouselService { get; }
        private IProductCategoryService _productCategoryService { get; }
        private IDiscountCategoryService _discountCategoryService { get; }
        private IBlogPostService _blogPostService { get; }

        private MessageModel _model;
        private Message _entity;

        #endregion


        #region

        public HomeController(IKeyValueContentService kvContentService, IMessageService messageService, ICarouselService carouselService, IProductCategoryService productCategoryService, IDiscountCategoryService discountCategoryService, IBlogPostService blogPostService)
        {
            _kvContentService = kvContentService;
            _messageService = messageService;
            _carouselService = carouselService;
            _productCategoryService = productCategoryService;
            _discountCategoryService = discountCategoryService;
            _blogPostService = blogPostService;

            _model = new MessageModel();
            _entity = new Message();

            LayoutHelper.PrepareLayout();
        }

        #endregion


        #region Methods

        public ActionResult Index()
        {
            ViewBag.Active = "Home";

            return View();
        }

        public ActionResult HeroCarouselSection()
        {
            List<Carousel> carousels = Task.Run(() => _carouselService.ListAsync()).Result
                .Where(x => x.IsDisplayed == true)
                .OrderBy(x => x.Priority)
                .ToList();

            return PartialView("_HeroCarousel", carousels);
        }

        public ActionResult MainCategoriesSection()
        {
            ViewBag.MainCategoryOneLink = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-one-link")).Result;
            ViewBag.MainCategoryOneImage = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-one-image")).Result;
            ViewBag.MainCategoryOneTitle = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-one-title")).Result;

            ViewBag.MainCategoryTwoLink = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-two-link ")).Result;
            ViewBag.MainCategoryTwoImage = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-two-image")).Result;
            ViewBag.MainCategoryTwoTitle = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-two-title")).Result;

            ViewBag.MainCategoryThreeLink = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-three-link ")).Result;
            ViewBag.MainCategoryThreeImage = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-three-image")).Result;
            ViewBag.MainCategoryThreeTitle = Task.Run(() => _kvContentService.GetValueByKeyAsync("@main-category-three-title")).Result;

            return PartialView("_MainCategories");
        }

        public ActionResult WomensNewestSection()
        {
            List<Product> productEntities = Task.Run(() => _productCategoryService.ListAsync()).Result
                .FirstOrDefault(x => x.Title.ToLower() == "women")
                .Products
                .OrderBy(x => x.CreateDateTime)
                .Take(10)
                .ToList();

            List<ProductModel> productModels = new List<ProductModel>();
            ProductModel model;

            foreach (var product in productEntities)
            {
                model = new ProductModel();
                ProductEntityToModel(product, ref model);
                productModels.Add(model);
            }

            ViewBag.WomensNewestPicture = Task.Run(() => _kvContentService.GetValueByKeyAsync("@womens-newest-section-picture")).Result;

            return PartialView("_WomensNewest", productModels);
        }

        public ActionResult DealsBannerSection()
        {
            var discountEntity = Task.Run(() => _discountCategoryService.ListAsync()).Result
                .Where(x => x.IsActive == true && x.ExpiryDateTime > DateTime.UtcNow)
                .OrderBy(x => x.Discount)
                .First();

            DiscountCategoryModel model = new DiscountCategoryModel();
            DiscountCategoryEntityToModel(discountEntity, ref model);

            return PartialView("_DealsBanner", model);
        }

        public ActionResult MensNewestSection()
        {
            List<Product> productEntities = Task.Run(() => _productCategoryService.ListAsync()).Result
                .FirstOrDefault(x => x.Title.ToLower() == "men")
                .Products
                .OrderBy(x => x.CreateDateTime)
                .Take(10)
                .ToList();

            List<ProductModel> productModels = new List<ProductModel>();
            ProductModel model;

            foreach (var product in productEntities)
            {
                model = new ProductModel();
                ProductEntityToModel(product, ref model);
                productModels.Add(model);
            }

            ViewBag.MensNewestPicture = Task.Run(() => _kvContentService.GetValueByKeyAsync("@mens-newest-section-picture")).Result;

            return PartialView("_MensNewest", productModels);
        }

        public ActionResult NewestBlogsSection()
        {
            List<BlogPost> blogPostEntities = Task.Run(() => _blogPostService.ListAsync()).Result
                .OrderBy(x => x.CreateDateTime)
                .Take(3)
                .ToList();

            List<BlogPostModel> blogPostModels = new List<BlogPostModel>();
            BlogPostModel model;

            foreach (var blog in blogPostEntities)
            {
                model = new BlogPostModel();
                BlogPostEntityToModel(blog, ref model);
                blogPostModels.Add(model);
            }

            ViewBag.BenifitsOneTitle = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-one-title")).Result;
            ViewBag.BenifitsOneDescription = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-one-description")).Result;
            ViewBag.BenifitsOneImage = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-one-image")).Result;

            ViewBag.BenifitsTwoTitle = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-two-title")).Result;
            ViewBag.BenifitsTwoDescription = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-two-description")).Result;
            ViewBag.BenifitsTwoImage = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-two-image")).Result;

            ViewBag.BenifitsThreeTitle = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-three-title")).Result;
            ViewBag.BenifitsThreeDescription = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-three-description")).Result;
            ViewBag.BenifitsThreeImage = Task.Run(() => _kvContentService.GetValueByKeyAsync("@benifits-three-image")).Result;

            return PartialView("_NewestBlogs", blogPostModels);
        }

        public async Task<ActionResult> Contact()
        {
            ViewBag.MapAddress = await _kvContentService.GetValueByKeyAsync("@map-address");
            ViewBag.ContactUsAboutText = await _kvContentService.GetValueByKeyAsync("@contact-us-about-text");
            ViewBag.ContactUSFormText = await _kvContentService.GetValueByKeyAsync("@contact-us-form-text");
            ViewBag.Address = await _kvContentService.GetValueByKeyAsync("@address");
            ViewBag.Phone = await _kvContentService.GetValueByKeyAsync("@phone");
            ViewBag.Email = await _kvContentService.GetValueByKeyAsync("@email");

            _model.Email = "";
            _model.Name = "";
            _model.MessageBody = "";

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Contact(MessageModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MapAddress = await _kvContentService.GetValueByKeyAsync("@map-address");
                ViewBag.ContactUsAboutText = await _kvContentService.GetValueByKeyAsync("@contact-us-about-text");
                ViewBag.ContactUSFormText = await _kvContentService.GetValueByKeyAsync("@contact-us-form-text");
                ViewBag.Address = await _kvContentService.GetValueByKeyAsync("@address");
                ViewBag.Phone = await _kvContentService.GetValueByKeyAsync("@phone");
                ViewBag.Email = await _kvContentService.GetValueByKeyAsync("@email");

                return View(model);
            }

            MessageModelToEntity(model, ref _entity);
            await _messageService.CreateAsync(_entity);

            return RedirectToAction("Index");
        }

        public ActionResult PrivacyPolicy()
        {
            ViewBag.Title = "Privacy Policy";

            return View();
        }

        public ActionResult TermsAndConditions()
        {
            ViewBag.Title = "Terms and Conditions";

            return View();
        }

        public ActionResult FAQ()
        {
            ViewBag.Title = "FAQ";

            return View();
        }

        #endregion


        #region Conversion

        private void MessageModelToEntity(MessageModel model, ref Message entity)
        {
            entity.Name = model.Name;
            entity.Email = model.Email;
            entity.Subject = $"Contact us Message - {System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] ?? "UNKNOWN IP"}";
            entity.MessageBody = model.MessageBody;
            entity.CreateDateTime = DateTime.UtcNow;
        }

        private void ProductEntityToModel(Product entity, ref ProductModel model)
        {
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.ShortDescription = entity.ShortDescription;
            model.Description = entity.Description;
            model.NoDiscountPrice = $"{entity.Price:0.00}";
            model.Price = model.NoDiscountPrice;

            if (entity.DiscountCategory != null)
                model.Price = $"{(entity.Price - entity.Price * (double)entity.DiscountCategory?.Discount):0.00}";

            model.Stock = entity.Stock;
            model.Images.Clear();
            model.Images.Add(entity.Image1);
            model.Images.Add(entity.Image2);
            model.Images.Add(entity.Image3);
            model.Images.Add(entity.Image4);
            model.Images.Add(entity.Image5);
            model.CategoryId = entity.ProductCategoryId;
            model.CategoryTitle = entity.ProductCategory?.Title ?? " - ";
            model.CreateDateTime = entity.CreateDateTime;
        }

        private void DiscountCategoryEntityToModel(DiscountCategory entity, ref DiscountCategoryModel model)
        {
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Discount = entity.Discount;
            model.UntilExpiration = DateTime.UtcNow - entity.ExpiryDateTime;
        }

        private void BlogPostEntityToModel(BlogPost entity, ref BlogPostModel model)
        {
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.ShortDescription = entity.ShortDescription;
            model.Content = entity.Content;
            model.Image = entity.Image;
            model.CategoryId = entity.BlogCategoryId;
            model.CategoryTitle = entity.BlogCategory?.Title ?? " - ";
            model.CreateDateTime = $"{entity.CreateDateTime:MMM d - yyyy}";
        }


        #endregion

    }
}