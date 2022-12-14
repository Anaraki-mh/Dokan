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
    public class TestimonialModel
    {
        #region Constructor

        public TestimonialModel()
        {

        }

        #endregion

        
        #region Properties
        public int Index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(50, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string FullName { get; set; }

        [Display(Name = "Position/Career")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(35, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Position { get; set; }

        [Display(Name = "Body")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(300, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Content { get; set; }

        [Display(Name = "Image")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image { get; set; }

        #endregion
    }
}