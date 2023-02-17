using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.WebEssentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dokan.Web.Helpers
{
    public static class LayoutHelper
    {
        #region Fields and Properties

        private static IKeyValueContentService _kvContentService { get; set; }
        private static IMenuService _menuService { get; set; }
        private static IProductCategoryService _productCategoryService { get; set; }

        private static bool _initialized = false;

        public static string Email { get; set; }
        public static string Phone { get; set; }
        public static string Address { get; set; }
        public static string HeaderSocialLinks { get; set; }
        public static string FooterSocialLinks { get; set; }
        public static string FooterWidgetOneTitle { get; set; }
        public static string FooterWidgetOneList { get; set; }
        public static string FooterWidgetTwoTitle { get; set; }
        public static string FooterWidgetTwoList { get; set; }

        public static List<Menu> MenuList { get; set; }
        public static List<Menu> MenuParentsList { get; set; }

        public static List<ProductCategory> ProductCategoryParentsList { get; set; }

        #endregion


        #region Constructor

        static LayoutHelper()
        {
            _kvContentService = DependencyResolver.Current.GetService<IKeyValueContentService>();
            _menuService = DependencyResolver.Current.GetService<IMenuService>();
            _productCategoryService = DependencyResolver.Current.GetService<IProductCategoryService>();
        }

        #endregion


        #region Methods

        public static void PrepareLayout()
        {
            if (_initialized)
                return;

            Phone = Task.Run(() => _kvContentService.GetValueByKeyAsync("@phone")).Result;
            Address = Task.Run(() => _kvContentService.GetValueByKeyAsync("@address")).Result;
            Email = Task.Run(() => _kvContentService.GetValueByKeyAsync("@email")).Result;
            HeaderSocialLinks = Task.Run(() => _kvContentService.GetValueByKeyAsync("@header-social-links")).Result;
            FooterSocialLinks = Task.Run(() => _kvContentService.GetValueByKeyAsync("@footer-social-links")).Result;
            FooterWidgetOneTitle = Task.Run(() => _kvContentService.GetValueByKeyAsync("@footer-widget-one-title")).Result;
            FooterWidgetOneList = Task.Run(() => _kvContentService.GetValueByKeyAsync("@footer-widget-one-list")).Result;
            FooterWidgetTwoTitle = Task.Run(() => _kvContentService.GetValueByKeyAsync("@footer-widget-two-title")).Result;
            FooterWidgetTwoList = Task.Run(() => _kvContentService.GetValueByKeyAsync("@footer-widget-two-list")).Result;

            MenuList = Task.Run(() => _menuService.ListAsync()).Result;
            MenuParentsList = Task.Run(() => _menuService.ListAsync()).Result
                .Where(x => x.ParentId == 0 || x.ParentId == null).OrderBy(x => x.Priority).ToList();
            ProductCategoryParentsList = Task.Run(() => _productCategoryService.ListAsync()).Result
                .Where(x => x.ParentId == 0 || x.ParentId == null).OrderBy(x => x.Priority).ToList();


            _initialized = true;
        }

        #endregion
    }
}