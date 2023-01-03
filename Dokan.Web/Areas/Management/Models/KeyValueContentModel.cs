using Dokan.Domain.Enums;
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
    public class KeyValueContentModel
    {
        #region Constructor

        public KeyValueContentModel()
        {

        }

        #endregion


        #region Properties
        public int Index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }


        [Display(Name = "Key")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(50, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ContentKey { get; set; }

        [AllowHtml]
        [Display(Name = "Value")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(1000, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ContentValue { get; set; }


        [Display(Name = "Description")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Description { get; set; }

        #endregion
    }
}