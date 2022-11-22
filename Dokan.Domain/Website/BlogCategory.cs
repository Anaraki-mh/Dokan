using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class BlogCategory : BaseEntity
    {

        #region Constructor

        public BlogCategory()
        {
            BlogPosts = new List<BlogPost>();
        }

        #endregion


        #region Properties

        [MaxLength(40)]
        public string Title { get; set; }

        public int Priority { get; set; }

        #endregion


        #region Relations

        public virtual List<BlogPost> BlogPosts { get; set; }

        public int? ParentId { get; set; }
        public virtual BlogCategory Parent { get; set; }

        #endregion

    }
}
