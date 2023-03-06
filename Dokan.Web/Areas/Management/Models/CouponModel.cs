using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace Dokan.Web.Areas.Management.Models
{
    public class CouponModel
    {
        #region Constructor

        public CouponModel()
        {
            CategoryIds = new List<int>();
            CategoryDropdown = new List<SelectListItem>();
        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }

        public int Index { get; set; }

        [Display(Name = "Title")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Display(Name = "Coupon Code")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(20, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Code { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDateTime { get; set; }

        [Display(Name = "Discount")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(0, 100, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Discount { get; set; }

        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(0, Double.PositiveInfinity, ErrorMessage = "{0} can not be less than {1}")]
        [Display(Name = "Usage Limit")]
        public int UsageLimit { get; set; }

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }


        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public List<int> CategoryIds { get; set; }
        public string CategoryTitles { get; set; }

        public List<SelectListItem> CategoryDropdown { get; set; }

        #endregion


        #region Conversion Helpers

        public static CouponModel EntityToModel(in Coupon entity, int index = 0)
        {
            var model = new CouponModel()
            {
                Index = index,
                Id = entity.Id,
                Title = entity.Title,
                Code = entity.Code,
                IsActive = entity.IsActive,
                Discount = entity.Discount,
                UsageLimit = entity.UsageLimit,
                UsageCount = entity.UsageCount,
                CategoryIds = entity.ProductCategories?.Select(c => c.Id).ToList(),
                CategoryTitles = String.Join(", ", entity.ProductCategories?.Select(c => c.Title).ToList()),
                ExpiryDateTime = entity.ExpiryDateTime,
                UpdateDateTime = entity.UpdateDateTime,
            };

            return model;
        }

        public static Coupon ModelToEntity(in CouponModel model, List<ProductCategory> productCategories)
        {
            var entity = new Coupon()
            {
                Id = model.Id,
                Title = model.Title,
                Code = model.Code,
                IsActive = model.IsActive,
                Discount = model.Discount,
                UsageLimit = model.UsageLimit,
                UsageCount = model.UsageCount,
                ExpiryDateTime = model.ExpiryDateTime,
                UpdateDateTime = model.UpdateDateTime,
            };

            foreach (var id in model.CategoryIds)
            {
                ProductCategory category = productCategories.FirstOrDefault(x => x.Id == id);
                entity.ProductCategories.Add(category);
            }

            return entity;
        }

        #endregion


        #region Preparation Helpers

        public static void PrepareDropdown(ref CouponModel model, List<ProductCategory> dropdownItemsList)
        {
            model.CategoryDropdown.Clear();

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