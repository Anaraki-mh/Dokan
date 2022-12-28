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
    public class CouponModel
    {
        #region Constructor

        public CouponModel()
        {
            CategoryIds = new List<int>();
            CategoryTitles = new List<string>();
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
        [Range(0,100, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Discount { get; set; }

        [Display(Name = "Usage Limit")]
        public int UsageLimit { get; set; }

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }


        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public List<int> CategoryIds { get; set; }

        public List<SelectListItem> CategoryDropdown { get; set; }

        #endregion
    }
}