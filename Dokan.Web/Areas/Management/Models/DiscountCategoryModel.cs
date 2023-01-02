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
    public class DiscountCategoryModel
    {
        #region Constructor

        public DiscountCategoryModel()
        {
            ProductIds = new List<int>();
            ProductDropdown = new List<SelectListItem>();
        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }

        [Display(Name = "Expiry date")]
        public DateTime ExpiryDateTime { get; set; }

        public int Index { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Display(Name = "Discount")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(0,100, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Discount { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public List<int> ProductIds { get; set; }

        public List<SelectListItem> ProductDropdown { get; set; }

        #endregion
    }
}