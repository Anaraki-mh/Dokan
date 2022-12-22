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
    public class CarouselModel
    {
        #region Constructor

        public CarouselModel()
        {
            TitleColor = "#ffffff";
            DescriptionColor = "#ffffff";
            ButtonOneFgColor = "#ffffff";
            ButtonTwoFgColor = "#ffffff";
            ButtonOneBgColor = "#000000";
            ButtonTwoBgColor = "#000000";
        }

        #endregion


        #region Properties

        public int index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }


        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image { get; set; }

        [Display(Name = "Display")]
        public bool IsDisplayed { get; set; }

        [Display(Name = "Description")]
        [MaxLength(80, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Description { get; set; }

        [Display(Name = "Button One")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")] 
        public string ButtonOne { get; set; }

        [Display(Name = "Button One Link")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string LinkOne { get; set; }

        [Display(Name = "Button Two")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ButtonTwo { get; set; }

        [Display(Name = "Button Two Link")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string LinkTwo { get; set; }

        [Display(Name = "Title Color")]
        [MaxLength(9, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string TitleColor { get; set; }

        [Display(Name = "Description Color")]
        [MaxLength(9, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string DescriptionColor { get; set; }

        [Display(Name = "Button One Background Color")]
        [MaxLength(9, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ButtonOneBgColor { get; set; }

        [Display(Name = "Button One Text Color")]
        [MaxLength(9, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ButtonOneFgColor { get; set; }

        [Display(Name = "Button Two Background Color")]
        [MaxLength(9, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ButtonTwoBgColor { get; set; }

        [Display(Name = "Button Two Text Color")]
        [MaxLength(9, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ButtonTwoFgColor { get; set; }


        #endregion
    }
}