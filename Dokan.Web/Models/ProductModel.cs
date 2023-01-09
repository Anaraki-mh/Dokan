using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace Dokan.Web.Models
{
    public class ProductModel
    {
        #region Constructor

        public ProductModel()
        {
            Images = new List<string>();
        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Create date")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        public string Price { get; set; }

        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Display(Name = "Images")]
        public List<string> Images { get; set; }


        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; }

        #endregion
    }
}