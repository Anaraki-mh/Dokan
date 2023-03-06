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
    public class ProductCommentModel
    {
        #region Constructor

        public ProductCommentModel()
        {

        }

        #endregion


        #region Properties
        public int Index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create date")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "Product Title")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ProductTitle { get; set; }

        public int ProductId { get; set; }
        public int ParentId { get; set; }

        [Display(Name = "Title")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Display(Name = "Username")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Username { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Comment Body")]
        [MaxLength(300, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Body { get; set; }

        [Display(Name = "Approval Status")]
        public bool IsApproved { get; set; }

        [Display(Name = "Rating")]
        [Range(0, 5, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Rating { get; set; }

        #endregion


        #region Conversion Helpers

        public static ProductCommentModel EntityToModel(in ProductComment entity, int index = 0)
        {
            var model = new ProductCommentModel()
            {
                Id = entity.Id,
                Index = index,
                ProductTitle = entity.Product?.Title,
                ProductId = entity.ProductId,
                Title = entity.Title,
                Username = entity.User?.UserName,
                UserId = entity.UserId,
                Body = entity.Body,
                IsApproved = entity.IsApproved,
                Rating = entity.Rating,
                CreateDateTime = entity.CreateDateTime,
            };

            return model;
        }

        public static ProductComment ModelToEntity(in ProductCommentModel model)
        {
            var entity = new ProductComment()
            {
                Id = model.Id,
                ProductId = model.ProductId,
                Title = model.Title,
                UserId = model.UserId,
                Body = model.Body,
                IsApproved = model.IsApproved,
                Rating = model.Rating,
                CreateDateTime = model.CreateDateTime,
            };

            return entity;
        }

        #endregion
    }
}