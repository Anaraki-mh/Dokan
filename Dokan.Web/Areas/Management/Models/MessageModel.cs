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
    }
}