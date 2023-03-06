using Dokan.Domain.Website;
using Microsoft.Owin.Security.Provider;
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
    public class BlogPostModel
    {
        #region Constructor

        public BlogPostModel()
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
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ShortDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Main Body")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public string Content { get; set; }

        [Display(Name = "Image")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; }

        public List<SelectListItem> CategoryDropdown { get; set; }

        #endregion


        #region Conversion Helpers

        public static BlogPostModel EntityToModel(in BlogPost entity, int index = 0)
        {
            var model = new BlogPostModel()
            {
                Index = index,
                Id = entity.Id,
                Title = entity.Title,
                ShortDescription = entity.ShortDescription,
                Content = entity.Content,
                Image = entity.Image,
                CategoryId = entity.BlogCategoryId,
                CategoryTitle = entity.BlogCategory?.Title ?? " - ",
                UpdateDateTime = entity.UpdateDateTime,
            };

            return model;
        }

        public static BlogPost ModelToEntity(in BlogPostModel model)
        {
            var entity = new BlogPost()
            {
                Id = model.Id,
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                Content = model.Content,
                Image = model.Image,
                BlogCategoryId = model.CategoryId,
                UpdateDateTime = model.UpdateDateTime,
            };

            return entity;
        }

        #endregion


        #region Preparation Helpers

        public static void PrepareDropdown(ref BlogPostModel model, List<BlogCategory> dropdownItemsList)
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