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
    public class BlogPostModel
    {
        #region Constructor

        public BlogPostModel()
        {

        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Create date")]
        public string CreateDateTime { get; set; }

        public int Index { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Main Body")]
        public string Content { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; }

        #endregion


        #region Conversion Helpers

        public static BlogPostModel EntityToModel(in BlogPost entity)
        {
            var model = new BlogPostModel()
            {
                Id = entity.Id,
                Title = entity.Title,
                ShortDescription = entity.ShortDescription,
                Content = entity.Content,
                Image = entity.Image,
                CategoryId = entity.BlogCategoryId,
                CategoryTitle = entity.BlogCategory?.Title ?? " - ",
                CreateDateTime = $"{entity.CreateDateTime:MMM d - yyyy}",
            };

            return model;
        }

        #endregion
    }
}