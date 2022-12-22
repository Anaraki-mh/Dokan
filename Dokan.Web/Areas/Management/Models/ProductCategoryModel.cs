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
    public class ProductCategoryModel
    {
        #region Constructor

        public ProductCategoryModel()
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
    }
}