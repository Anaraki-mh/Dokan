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
        public string CreateDateTime { get; set; }

        public int ProductId { get; set; }
        public int ParentId { get; set; }

        [Display(Name = "Username")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Username { get; set; }

        public string UserId { get; set; }

        public string UserProfilePic { get; set; }

        [Display(Name = "Comment Body")]
        [MaxLength(300, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Body { get; set; }

        [Display(Name = "Rating")]
        [Range(0, 5, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Rating { get; set; }

        #endregion


        #region Conversion Helpers

        public static ProductCommentModel EntityToModel(in ProductComment entity)
        {
            var model = new ProductCommentModel()
            {
                Id = entity.Id,
                Username = entity.User?.UserName,
                UserId = entity.UserId,
                UserProfilePic = entity.User?.UserInformation?.ProfilePicture,
                ProductId = entity.ProductId,
                Body = entity.Body,
                Rating = entity.Rating,
                CreateDateTime = $"{entity.CreateDateTime:MMM d yyyy}",
            };

            return model;
        }

        public static ProductComment ModelToEntity(in ProductCommentModel model)
        {
            var entity = new ProductComment()
            {
                Id = model.Id,
                UserId = model.UserId,
                ProductId = model.ProductId,
                Body = model.Body,
                Rating = model.Rating,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
            };

            return entity;
        }

        #endregion
    }
}