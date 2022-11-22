using Dokan.Domain.BaseData;
using Dokan.Domain.UsersAndRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class BlogComment : BaseComment
    {
        [MaxLength(100)]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }

        public int? ParentId { get; set; }
        public virtual BlogComment Parent { get; set; }
    }
}
