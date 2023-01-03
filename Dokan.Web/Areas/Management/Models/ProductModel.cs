using Dokan.Domain.Website;
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
    public class ProductModel
    {
        #region Constructor

        public ProductModel()
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
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ShortDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(3000, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(0, Double.PositiveInfinity, ErrorMessage = "{0} can not be less than {1}")]
        public double Price { get; set; }

        [Display(Name = "Stock")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(-1, int.MaxValue, ErrorMessage = "{0} can not be less than {1}")]
        public int Stock { get; set; }

        [Display(Name = "Image 1")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image1 { get; set; }

        [Display(Name = "Image 2")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image2 { get; set; }

        [Display(Name = "Image 3")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image3 { get; set; }

        [Display(Name = "Image 4")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image4 { get; set; }

        [Display(Name = "Image 5")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Image5 { get; set; }


        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; }

        public List<SelectListItem> CategoryDropdown { get; set; }

        #endregion
    }
}