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
    public class DiscountCategoryModel
    {
        #region Constructor

        public DiscountCategoryModel()
        {

        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Time Left Until Expiration")]
        public TimeSpan UntilExpiration { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Display(Name = "Discount")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [Range(0, 100, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Discount { get; set; }

        #endregion


        #region Conversion Helpers

        public static DiscountCategoryModel EntityToModel(in DiscountCategory entity)
        {
            var model = new DiscountCategoryModel()
            {
                Id = entity.Id,
                Title = entity.Title,
                Discount = entity.Discount,
                UntilExpiration = DateTime.UtcNow - entity.ExpiryDateTime,
            };

            return model;
        }

        #endregion
    }
}