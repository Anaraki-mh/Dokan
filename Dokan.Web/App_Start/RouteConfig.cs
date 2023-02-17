using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dokan.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Management",
                url: "Management/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Dokan.Web.Areas.Management" }
            );

            routes.MapRoute(
            name: "ProductCategory",
            url: "Products/Category/{id}/{title}",
            defaults: new { controller = "Products", action = "Category", id = UrlParameter.Optional },
            namespaces: new[] { "Dokan.Web.Controllers" }
            );

            routes.MapRoute(
            name: "BlogCategory",
            url: "Blog/Category/{id}/{title}",
            defaults: new { controller = "Blog", action = "Category", id = UrlParameter.Optional },
            namespaces: new[] { "Dokan.Web.Controllers" }
            );

            routes.MapRoute(
            name: "ProductDetails",
            url: "Products/Details/{id}/{title}",
            defaults: new { controller = "Products", action = "Details", title = UrlParameter.Optional },
            namespaces: new[] { "Dokan.Web.Controllers" }
            );

            routes.MapRoute(
            name: "BlogDetails",
            url: "Blog/Details/{id}/{title}",
            defaults: new { controller = "Blog", action = "Details", title = UrlParameter.Optional },
            namespaces: new[] { "Dokan.Web.Controllers" }
            );


            routes.MapRoute(
            name: "Cart",
            url: "Cart",
            defaults: new { controller = "Cart", Action = "ShoppingCart" },
            namespaces: new[] { "Dokan.Web.Controllers" }
            );

            routes.MapRoute(
            name: "Home",
            url: "{action}",
            defaults: new { controller = "Home" },
            namespaces: new[] { "Dokan.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Dokan.Web.Controllers" }
            );
        }
    }
}
