using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dokan.Domain.Website;

namespace Dokan.Domain.UsersAndRoles
{
    public class User : IdentityUser
    {

        #region Constructor

        public User()
        {
            ProductComments = new List<ProductComment>();
            BlogComments = new List<BlogComment>();
            Orders = new List<Order>();
            UsedCoupons = new List<Coupon>();
        }

        #endregion


        #region Methods

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        #endregion


        #region Relations

        public virtual List<ProductComment> ProductComments { get; set; }
        public virtual List<BlogComment> BlogComments { get; set; }
        public virtual List<Order> Orders { get; set; }
        public virtual List<Coupon> UsedCoupons { get; set; }

        #endregion
    }
}
