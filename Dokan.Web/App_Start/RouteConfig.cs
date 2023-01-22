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
            name: "Category",
            url: "{controller}/Category/{id}/{title}",
            namespaces: new[] { "Dokan.Web.Controllers" }
            );

            // merge the two
            routes.MapRoute(
            name: "ProductDetails",
            url: "Products/Details/{id}/{title}",
            defaults: new { controller = "Product", action = "Details", id = UrlParameter.Optional },
            namespaces: new[] { "Dokan.Web.Controllers" }
            );

            routes.MapRoute(
            name: "Details",
            url: "{controller}/Details/{id}/{title}",
            defaults: new { controller = "Blog", action = "Details", id = UrlParameter.Optional },
            namespaces: new[] { "Dokan.Web.Controllers" }
            );

            routes.MapRoute(
            name: "Home",
            url: "{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
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
