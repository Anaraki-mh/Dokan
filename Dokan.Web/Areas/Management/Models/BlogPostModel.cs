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
    }
}