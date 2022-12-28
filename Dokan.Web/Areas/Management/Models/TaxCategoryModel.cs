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
        [Range(0,100, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Tax { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public List<int> CategoryIds { get; set; }

        public List<SelectListItem> CategoryDropdown { get; set; }

        #endregion
    }
}