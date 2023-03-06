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
    public class MessageModel
    {
        #region Constructor

        public MessageModel()
        {

        }

        #endregion


        #region Properties
        public int Index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create date")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "Name")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Email { get; set; }

        [Display(Name = "Subject")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Subject { get; set; }

        [Display(Name = "Message Body")]
        [MaxLength(400, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string MessageBody { get; set; }

        #endregion


        #region Conversion Helpers

        public static MessageModel EntityToModel(in Message entity, int index = 0)
        {
            var model = new MessageModel()
            {
                Id = entity.Id,
                Index = index,
                Name = entity.Name,
                Email = entity.Email,
                Subject = entity.Subject,
                MessageBody = entity.MessageBody,
                CreateDateTime = entity.CreateDateTime,
            };

            return model;
        }

        public static Message ModelToEntity(in MessageModel model)
        {
            var entity = new Message()
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                Subject = model.Subject,
                MessageBody = model.MessageBody,
                CreateDateTime = model.CreateDateTime,
            };

            return entity;
        }


        #endregion
    }
}