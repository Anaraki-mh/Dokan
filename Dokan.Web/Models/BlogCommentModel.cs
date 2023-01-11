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
    public class BlogCommentModel
    {
        #region Constructor

        public BlogCommentModel()
        {

        }

        #endregion


        #region Properties
        public int Index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create date")]
        public DateTime CreateDateTime { get; set; }

        public int BlogPostId { get; set; }
        public int ParentId { get; set; }

        [Display(Name = "Username")]
        [MaxLength(40, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Username { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Comment Body")]
        [MaxLength(300, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Body { get; set; }

        [Display(Name = "Rating")]
        [Range(0,5, ErrorMessage = "{0} can not be less than {1} and greater {2}")]
        public int Rating { get; set; }

        #endregion
    }
}