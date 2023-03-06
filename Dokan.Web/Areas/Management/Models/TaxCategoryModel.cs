using Dokan.Domain.Website;
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
    public class TaxCategoryModel
    {
        #region Constructor

        public TaxCategoryModel()
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
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Display(Name = "Tax")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(0, 100, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Tax { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public List<int> CategoryIds { get; set; }
        public string CategoryTitles { get; set; }

        public List<SelectListItem> CategoryDropdown { get; set; }

        #endregion


        #region Conversion Helpers

        public static TaxCategoryModel EntityToModel(in TaxCategory entity, int index = 0)
        {
            var model = new TaxCategoryModel()
            {
                Index = index,
                Id = entity.Id,
                Title = entity.Title,
                Tax = entity.Tax,
                CategoryIds = entity.ProductCategories?.Select(c => c.Id).ToList(),
                CategoryTitles = String.Join(", ", entity.ProductCategories?.Select(c => c.Title).ToList()),
                UpdateDateTime = entity.UpdateDateTime,
            };

            return model;
        }

        public static TaxCategory ModelToEntity(in TaxCategoryModel model, List<ProductCategory> productCategories)
        {
            var entity = new TaxCategory()
            {
                Id = model.Id,
                Title = model.Title,
                Tax = model.Tax,
                UpdateDateTime = model.UpdateDateTime,
            };

            foreach (var id in model.CategoryIds)
            {
                var category = productCategories.FirstOrDefault(x => x.Id == id);
                entity.ProductCategories.Add(category);
            }

            return entity;
        }

        #endregion


        #region Preparation Helpers

        public static void PrepareDropdown(ref TaxCategoryModel model, List<ProductCategory> dropdownItemsList)
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