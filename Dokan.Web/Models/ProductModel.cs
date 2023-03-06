using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace Dokan.Web.Models
{
    public class ProductModel
    {
        #region Constructor

        public ProductModel()
        {
            Images = new List<string>();
        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Create date")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        public string Price { get; set; }

        [Display(Name = "Price Without Discount")]
        public string NoDiscountPrice { get; set; }

        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Display(Name = "Images")]
        public List<string> Images { get; set; }

        [Display(Name = "Rating")]
        public double Rating { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; }

        #endregion


        #region Conversion Helpers

        public static ProductModel EntityToModel(in Product entity)
        {
            var tax = entity.ProductCategory.TaxCategory?.Tax ?? 0;

            var model = new ProductModel()
            {
                Id = entity.Id,
                Title = entity.Title,
                ShortDescription = entity.ShortDescription,
                Description = entity.Description,
                NoDiscountPrice = $"{entity.Price + entity.Price * (double)tax:0.00}",
                Price = $"{entity.Price + entity.Price * (double)tax:0.00}",
                Stock = entity.Stock,
                Images = new List<string>
                {
                    entity.Image1,
                    entity.Image2,
                    entity.Image3,
                    entity.Image4,
                    entity.Image5,
                },
                Rating = 5,
                CategoryId = entity.ProductCategoryId,
                CategoryTitle = entity.ProductCategory?.Title ?? " - ",
                CreateDateTime = entity.CreateDateTime,
            };

            if (entity.DiscountCategory != null)
                model.Price = $"{(entity.Price + entity.Price * (double)entity.ProductCategory.TaxCategory?.Tax - entity.Price * (double)entity.DiscountCategory?.Discount):0.00}";

            if (entity.ProductComments.Count > 0)
                model.Rating = (double)entity.ProductComments?.Average(x => x.Rating);

            return model;
        }

        #endregion
    }
}