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
    public class MenuModel
    {
        #region Constructor

        public MenuModel()
        {
            ParentDropdown = new List<SelectListItem>();
        }

        #endregion


        #region Properties
        public int Index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }


        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(45, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Link { get; set; }

        [Display(Name = "Display")]
        public bool IsDisplayed { get; set; }

        [Display(Name = "Display priority")]
        public int? Priority { get; set; }
        

        [Display(Name = "Parent")]
        public int? ParentId { get; set; }

        public string ParentTitle { get; set; }

        public List<SelectListItem> ParentDropdown { get; set; }

        #endregion
    }
}