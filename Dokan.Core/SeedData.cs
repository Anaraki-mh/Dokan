using Dokan.Core.Database;
using Dokan.Domain.UsersAndRoles;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dokan.Domain.Website;
using System.Net.Http.Headers;

namespace Dokan.Core
{
    public class SeedData
    {
        #region Fields and Properties

        public DokanContext Context { get; set; }
        public UserStore<User> UserStore { get; set; }
        public UserManager<User> UserManager { get; set; }

        #endregion


        #region Contructor

        public SeedData(in DokanContext context)
        {
            Context = context;
            UserStore = new UserStore<User>(context);
            UserManager = new UserManager<User>(UserStore);
        }

        #endregion


        #region Methods

        public void Roles()
        {
            if (Context.Users.Count() > 0)
                return;

            Context.Roles.Add(new IdentityRole
            {
                Name = "user",
            });
            Context.Roles.Add(new IdentityRole
            {
                Name = "admin",
            });

            SaveChanges();
        }

        public void Users()
        {
            if (Context.Users.Count() > 0)
                return;

            var user = new User
            {
                Id = "00000000-0000-0000-0000-000000000000",
                UserName = "Guest",
                Email = "Guest@Dokan.com",
                UserInformation = new UserInformation(),
                EmailConfirmed = true,
            };
            user.Roles.Add(new IdentityUserRole { RoleId = Context.Roles.FirstOrDefault(x => x.Name == "user").Id });
            UserManager.Create(user, "Guest@Dokan.com");
        }

        public void Menu()
        {
            if (Context.Menus.Count() > 0)
                return;

            Context.Menus.AddRange(new List<Menu>
            {
                new Menu
                {
                    Title = "Home",
                    Link = "/",
                    Priority = 1,
                    IsDisplayed = true,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                },
                new Menu
                {
                    Title = "Shop",
                    Link = "/Products/Index",
                    Priority = 1,
                    IsDisplayed = true,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                },
                new Menu
                {
                    Title = "Blog",
                    Link = "/Blog/Index",
                    Priority = 1,
                    IsDisplayed = true,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                },
                new Menu
                {
                    Title = "Contact",
                    Link = "/Contact",
                    Priority = 1,
                    IsDisplayed = true,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                }
            });

            SaveChanges();
        }

        public void KeyValueContent()
        {
            if (Context.KeyValueContents.Count() > 0)
                return;

            Context.KeyValueContents.AddRange(new List<KeyValueContent>
            {
                new KeyValueContent
                {
                    ContentKey = "@map-address",
                    ContentValue = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d48158.305462977965!2d-74.13283844036356!3d41.02757295168286!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x89c2e440473470d7%3A0xcaf503ca2ee57958!2sSaddle%20River%2C%20NJ%2007458%2C%20USA!5e0!3m2!1sen!2sbd!4v1575917275626!5m2!1sen!2sbd",
                    Description= "Link of the google map embed (src)",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@contact-us-about-text",
                    ContentValue = "Dokan is an Ecommerce website made with love by Anaraki-Mh. The front-end bootstrap templates used (and modified) are Fashi and PurpleAdmin.",
                    Description= "The about us text in contact us page",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@contact-us-form-text",
                    ContentValue = "Our staff will call back later and answer your questions.",
                    Description= "The text above the form in contact us page",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@address",
                    ContentValue = "60-49 Road 11378 New York",
                    Description= "The address of the shop",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@phone",
                    ContentValue = "+65 11.188.888",
                    Description= "Phone number of the shop or the owner",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@email",
                    ContentValue = "Dokan@gmail.com",
                    Description= "Email address of the shop or the owner",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@header-social-links",
                    ContentValue = "<a href=\"#\"><i class=\"ti-facebook\"></i></a>\r\n" +
                                   "<a href=\"#\"><i class=\"ti-twitter-alt\"></i></a>\r\n" +
                                   "<a href=\"#\"><i class=\"ti-linkedin\"></i></a>\r\n" +
                                   "<a href=\"#\"><i class=\"ti-pinterest\"></i></a>",
                    Description= "The social media links in the header (HTML)",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@footer-social-links",
                    ContentValue = "<a href=\"#\"><i class=\"ti-facebook\"></i></a>\r\n" +
                                   "<a href=\"#\"><i class=\"ti-twitter-alt\"></i></a>\r\n" +
                                   "<a href=\"#\"><i class=\"ti-linkedin\"></i></a>\r\n" +
                                   "<a href=\"#\"><i class=\"ti-pinterest\"></i></a>",
                    Description= "The social media links in the header (HTML)",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@footer-widget-one-title",
                    ContentValue = "Information",
                    Description= "Footer second column title",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@footer-widget-one-list",
                    ContentValue = "<li><a href=\"#\">About Us</a></li>\r\n" +
                                    "<li><a href=\"#\">Contact Us</a></li>\r\n" +
                                    "<li><a href=\"#\">Terms Of Service</a></li>\r\n" +
                                    "<li><a href=\"#\">Privacy Policy</a></li>\r\n" +
                                    "<li><a href=\"#\">FAQ</a></li>",
                    Description= "Footer second column list (HTML)",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@footer-widget-two-title",
                    ContentValue = "Account",
                    Description= "Footer third column title",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@footer-widget-two-list",
                    ContentValue = "<li><a href=\"#\">Log In / Sign Up</a></li>\r\n" +
                                    "<li><a href=\"#\">My Account</a></li>\r\n" +
                                    "<li><a href=\"#\">Shopping Cart</a></li>\r\n" +
                                    "<li><a href=\"#\">Shop</a></li>",
                    Description= "Footer third column list (HTML)",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-one-link",
                    ContentValue = "/Products/Men",
                    Description= "Link of the first main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-one-image",
                    ContentValue = "men.jpg",
                    Description= "Image of the first main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-one-title",
                    ContentValue = "Men",
                    Description= "Title of the first main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-two-link",
                    ContentValue = "/Products/Women",
                    Description= "Link of the second main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-two-image",
                    ContentValue = "women.jpg",
                    Description= "Image of the second main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-two-title",
                    ContentValue = "Women",
                    Description= "Title of the second main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-three-link",
                    ContentValue = "/Products/Kids",
                    Description= "Link of the third main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-three-image",
                    ContentValue = "kids.jpg",
                    Description= "Image of the third main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@main-category-three-title",
                    ContentValue = "Kids",
                    Description= "Title of the third main category",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@mens-newest-section-picture",
                    ContentValue = "mennewest.jpg",
                    Description= "The image background of the men's newest section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@womens-newest-section-picture",
                    ContentValue = "womennewest.jpg",
                    Description= "The image background of the women's newest section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-one-title",
                    ContentValue = "FREE SHIPPING",
                    Description= "Title of the first item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-one-description",
                    ContentValue = "For all order over 99$",
                    Description= "Description of the first item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-one-image",
                    ContentValue = "benifitsone.png",
                    Description= "Image of the first item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-two-title",
                    ContentValue = "DELIVERY ON TIME",
                    Description= "Title of the second item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-two-description",
                    ContentValue = "Without any expetions",
                    Description= "Description of the second item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-two-image",
                    ContentValue = "benifitstwo.png",
                    Description= "Image of the second item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-three-title",
                    ContentValue = "SECURE PAYMENT",
                    Description= "Title of the third item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-three-description",
                    ContentValue = "100% secure payment",
                    Description= "Description of the third item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@benifits-three-image",
                    ContentValue = "benifitsthree.png",
                    Description= "Image of the third item in benifits section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
                new KeyValueContent
                {
                    ContentKey = "@deals-banner",
                    ContentValue = "dealsbanner.jpg",
                    Description= "Backgoround of the deals section",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                },
            });

            SaveChanges();
        }

        public void Carousel()
        {
            if (Context.Carousels.Count() > 0)
                return;

            Context.Carousels.AddRange(new List<Carousel>
            {
                new Carousel
                {
                    IsDisplayed = true,
                    Priority = 1,
                    Image = "carouselone.jpg",
                    Title = "Discount",
                    TitleColor = "black",
                    Description = "Up to 30% discount on all products!",
                    DescriptionColor= "black",
                    ButtonOne = "SHOP NOW",
                    ButtonOneBgColor = "#e7ab3c",
                    ButtonOneFgColor = "white",
                    LinkOne = "/Products",
                    UpdateDateTime = DateTime.UtcNow,
                },
                new Carousel
                {
                    IsDisplayed = true,
                    Priority = 1,
                    Image = "carouseltwo.jpg",
                    Title = "Black Friday",
                    TitleColor = "black",
                    Description = "Up to 50% discount on all products!",
                    DescriptionColor= "black",
                    ButtonOne = "SHOP NOW",
                    ButtonOneBgColor = "#e7ab3c",
                    ButtonOneFgColor = "white",
                    LinkOne = "/Products",
                    UpdateDateTime = DateTime.UtcNow,
                },
                new Carousel
                {
                    IsDisplayed = true,
                    Priority = 1,
                    Image = "carouselthree.jpg",
                    Title = "New Products",
                    TitleColor = "black",
                    Description = "300 new products added in all categories!",
                    DescriptionColor= "black",
                    ButtonOne = "SHOP NOW",
                    ButtonOneBgColor = "#e7ab3c",
                    ButtonOneFgColor = "white",
                    LinkOne = "/Products",
                    UpdateDateTime = DateTime.UtcNow,
                },
            });

            SaveChanges();
        }

        public void Testimonials()
        {
            if (Context.Testimonials.Count() > 0)
                return;

            // to be added and completed
        }

        public void ProductCategories()
        {
            if (Context.ProductCategories.Count() > 0)
                return;

            Context.ProductCategories.AddRange(new List<ProductCategory>
            {
                new ProductCategory
                {
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    Priority = 1,
                    Title = "Men",
                },
                new ProductCategory
                {
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    Priority = 2,
                    Title = "Women",
                },
                new ProductCategory
                {
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    Priority = 3,
                    Title = "Kids",
                },
            });

            SaveChanges();
        }

        public void Products()
        {
            if (Context.Products.Count() > 0)
                return;

            Context.Products.AddRange(new List<Product>
            {
                new Product
                {
                    Title = "Shirt",
                    Stock = 99,
                    Price = 19.25,
                    Description= "Shirt for men who like shirts...",
                    ShortDescription = "A shirt for men",
                    Image1 = "productone.jpg",
                    Image2 = "producttwo.jpg",
                    Image3 = "productthree.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Men").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
                new Product
                {
                    Title = "Jeens",
                    Stock = 99,
                    Price = 40,
                    Description= "Jeens for men who like jeens...",
                    ShortDescription = "A jeens for men",
                    Image1 = "productthree.jpg",
                    Image2 = "productfour.jpg",
                    Image3 = "productfive.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Men").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
                new Product
                {
                    Title = "Hat",
                    Stock = 99,
                    Price = 25,
                    Description= "Hat for men who like hats...",
                    ShortDescription = "A hat for men",
                    Image1 = "productsix.jpg",
                    Image2 = "productseven.jpg",
                    Image3 = "producteight.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Men").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
                new Product
                {
                    Title = "Shirt",
                    Stock = 99,
                    Price = 44,
                    Description= "Shirt for women who like shirts...",
                    ShortDescription = "A shirt for women",
                    Image1 = "productfour.jpg",
                    Image2 = "productfive.jpg",
                    Image3 = "productseven.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Women").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
                new Product
                {
                    Title = "Jeens",
                    Stock = 99,
                    Price = 15.50,
                    Description= "Jeens for women who like jeens...",
                    ShortDescription = "A jeens for women",
                    Image1 = "producttwelve.jpg",
                    Image2 = "productten.jpg",
                    Image3 = "productnine.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Women").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
                new Product
                {
                    Title = "Hat",
                    Stock = 99,
                    Price = 33,
                    Description= "Hat for women who like hats...",
                    ShortDescription = "A hat for women",
                    Image1 = "productsix.jpg",
                    Image2 = "producteight.jpg",
                    Image3 = "producteleven.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Women").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
                new Product
                {
                    Title = "Shirt",
                    Stock = 99,
                    Price = 28.75,
                    Description= "Shirt for kids who like shirts...",
                    ShortDescription = "A shirt for men",
                    Image1 = "productseven.jpg",
                    Image2 = "productnine.jpg",
                    Image3 = "productsix.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Kids").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
                new Product
                {
                    Title = "Jeens",
                    Stock = 99,
                    Price = 19,
                    Description= "Jeens for kids who like jeens...",
                    ShortDescription = "A jeens for men",
                    Image1 = "productten.jpg",
                    Image2 = "productfive.jpg",
                    Image3 = "producteight.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Kids").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
                new Product
                {
                    Title = "Hat",
                    Stock = 99,
                    Price = 25,
                    Description= "Hat for kids who like hats...",
                    ShortDescription = "A hat for men",
                    Image1 = "producttwelve.jpg",
                    Image2 = "productone.jpg",
                    Image3 = "productthree.jpg",
                    ProductCategoryId = Context.ProductCategories.FirstOrDefault(x => x.Title == "Kids").Id,
                    CreateDateTime= DateTime.UtcNow,
                    UpdateDateTime= DateTime.UtcNow,
                },
            });

            SaveChanges();
        }

        public void BlogCategories()
        {
            if (Context.BlogCategories.Count() > 0)
                return;

            Context.BlogCategories.AddRange(new List<BlogCategory>
            {
                new BlogCategory
                {
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    Priority = 1,
                    Title = "News",
                },
            });

            SaveChanges();
        }

        public void BlogPosts()
        {
            if (Context.BlogPosts.Count() > 0)
                return;

            Context.BlogPosts.AddRange(new List<BlogPost>
            {
                new BlogPost
                {
                    Title = "Latest News!",
                    ShortDescription = "The latest news on the internet",
                    Content = "So many new things have happened in the past few days and its unbelievable!",
                    Image = "blogpostone.jpg",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    BlogCategoryId = Context.BlogCategories.FirstOrDefault(x => x.Title == "News").Id,
                },
                new BlogPost
                {
                    Title = "Best News!",
                    ShortDescription = "The best news on the internet",
                    Content = "So many new things have happened in the past few days and its unbelievable!",
                    Image = "blogposttwo.jpg",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    BlogCategoryId = Context.BlogCategories.FirstOrDefault(x => x.Title == "News").Id,
                },
                new BlogPost
                {
                    Title = "Exciting News!",
                    ShortDescription = "The exciting news on the internet",
                    Content = "So many new things have happened in the past few days and its unbelievable!",
                    Image = "blogpostthree.jpg",
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    BlogCategoryId = Context.BlogCategories.FirstOrDefault(x => x.Title == "News").Id,
                }
            });

            SaveChanges();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        #endregion
    }
}
