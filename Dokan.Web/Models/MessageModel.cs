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
    public class MessageModel
    {
        #region Constructor

        public MessageModel()
        {

        }

        #endregion


        #region Properties

        public int Id { get; set; }

        [Display(Name = "Create date")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Email { get; set; }

        [Display(Name = "Message Body")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(400, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string MessageBody { get; set; }

        #endregion


        #region Conversion Helpers

        public static Message ModelToEntity(in MessageModel model)
        {
            var entity = new Message()
            {
                Name = model.Name,
                Email = model.Email,
                Subject = $"Contact us Message - {System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] ?? "UNKNOWN IP"}",
                MessageBody = model.MessageBody,
                CreateDateTime = DateTime.UtcNow,
            };

            return entity;
        }

        #endregion
    }
}