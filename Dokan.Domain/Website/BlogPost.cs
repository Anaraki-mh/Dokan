using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dokan.Domain.Website
{
    public class BlogPost : BaseEntity
    {
        #region Constructor

        public BlogPost()
        {
            BlogComments = new List<BlogComment>();
        }

        #endregion


        #region Properties

        [MaxLength(75)]
        public string Title { get; set; }

        [MaxLength(30)]
        public string Image { get; set; }

        [MaxLength(75)]
        public string ShortDescription { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        #endregion


        #region Relations

        public int BlogCategoryId { get; set; }
        public virtual BlogCategory BlogCategory { get; set; }

        public virtual List<BlogComment> BlogComments { get; set; }

        #endregion
    }
}
