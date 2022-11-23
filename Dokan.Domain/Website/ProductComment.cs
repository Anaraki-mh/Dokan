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
    public class ProductComment : BaseComment
    {
        #region Relations

        [MaxLength(100)]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int? ParentId { get; set; }
        public virtual ProductComment Parent { get; set; }

        #endregion
    }
}
