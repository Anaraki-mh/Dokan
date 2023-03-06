using Dokan.Domain.Website;
using Microsoft.Owin.Security.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace Dokan.Web.Areas.Management.Models
{
    public class ProductModel
    {
        #region Constructor

        public ProductModel()
        {
            CategoryDropdown = new List<SelectListItem>();
        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }

        public int Index { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Display(Name = "Short Description")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ShortDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(3000, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(0, Double.PositiveInfinity, ErrorMessage = "{0} can not be less than {1}")]
        public double Price { get; set; }

        [Display(Name = "Stock")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(-1, int.MaxValue, ErrorMessage = "{0} can not be less than {1}")]
        public int Stock { get; set; }

        [Display(Name = "Image 1")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image1 { get; set; }

        [Display(Name = "Image 2")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image2 { get; set; }

        [Display(Name = "Image 3")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image3 { get; set; }

        [Display(Name = "Image 4")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image4 { get; set; }

        [Display(Name = "Image 5")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image5 { get; set; }


        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; }

        public List<SelectListItem> CategoryDropdown { get; set; }

        #endregion


        #region Conversion Helpers

        public static ProductModel EntityToModel(in Product entity, int index = 0)
        {
            var model = new ProductModel()
            {
                Index = index,
                Id = entity.Id,
                Title = entity.Title,
                ShortDescription = entity.ShortDescription,
                Description = entity.Description,
                Price = entity.Price,
                Stock = entity.Stock,
                Image1 = entity.Image1,
                Image2 = entity.Image2,
                Image3 = entity.Image3,
                Image4 = entity.Image4,
                Image5 = entity.Image5,
                CategoryId = entity.ProductCategoryId,
                CategoryTitle = entity.ProductCategory?.Title ?? " - ",
                UpdateDateTime = entity.UpdateDateTime,
            };

            return model;
        }

        public static Product ModelToEntity(in ProductModel model)
        {
            var entity = new Product()
            {
                Id = model.Id,
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                Image1 = model.Image1,
                Image2 = model.Image2,
                Image3 = model.Image3,
                Image4 = model.Image4,
                Image5 = model.Image5,
                ProductCategoryId = model.CategoryId,
                UpdateDateTime = model.UpdateDateTime,
            };
            return entity;
        }

        #endregion


        #region Preparation Helpers

        public static void PrepareDropdown(ref ProductModel model, List<ProductCategory> dropdownItemsList)
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