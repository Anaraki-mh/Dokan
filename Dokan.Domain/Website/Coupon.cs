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
    public class Coupon : BaseEntity
    {
        #region Constructor

        public Coupon()
        {
            ProductCategories = new List<ProductCategory>();
            Users = new List<User>();
            Orders = new List<Order>();
        }

        #endregion


        #region Properties

        [MaxLength(75)]
        public string Title { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiryDateTime { get; set; }

        public int Discount { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }

        #endregion


        #region Relations

        public virtual List<ProductCategory> ProductCategories { get; set; }
        public virtual List<User> Users { get; set; }
        public virtual List<Order> Orders { get; set; }

        #endregion
    }
}
