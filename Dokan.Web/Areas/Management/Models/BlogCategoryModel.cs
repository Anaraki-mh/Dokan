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
    public class BlogCategoryModel
    {
        #region Constructor

        public BlogCategoryModel()
        {
            ParentDropdown = new List<SelectListItem>(new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text= "Select an item...",
                    Value = "",
                    Selected = true
                }
            });
        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }

        public int index { get; set; }

        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Display(Name = "Display priority")]
        public int? Priority { get; set; }

        [Display(Name = "Parent Category")]
        public int? ParentId { get; set; }

        public string ParentTitle { get; set; }

        public List<SelectListItem> ParentDropdown { get; set; }

        #endregion


        #region Conversion Helpers

        public static BlogCategoryModel EntityToModel(in BlogCategory entity, int index = 0)
        {
            var model = new BlogCategoryModel()
            {
                Id = entity.Id,
                index = index,
                Title = entity.Title,
                Priority = entity.Priority,
                ParentId = entity.ParentId,
                ParentTitle = entity.Parent?.Title ?? " - ",
                UpdateDateTime = entity.UpdateDateTime,
            };

            return model;
        }

        public static BlogCategory ModelToEntity(in BlogCategoryModel model)
        {
            var entity = new BlogCategory()
            {
                Id = model.Id,
                Title = model.Title,
                Priority = model.Priority ?? 0,
                ParentId = model.ParentId,
            };

            return entity;
        }

        #endregion


        #region Preparation Helpers

        public static void PrepareDropdown(ref BlogCategoryModel model, List<BlogCategory> dropdownItemsList)
        {
            model.ParentDropdown.Clear();

            model.ParentDropdown.Add(new SelectListItem()
            {
                Text = "Select an item...",
                Value = "",
            });

            int modelId = model.Id;
            dropdownItemsList.Remove(dropdownItemsList.FirstOrDefault(x => x.Id == modelId));

            foreach (var entity in dropdownItemsList)
            {
                model.ParentDropdown.Add(new SelectListItem()
                {
                    Text = entity.Title,
                    Value = entity.Id.ToString(),
                });
            }
        }

        #endregion
    }
}