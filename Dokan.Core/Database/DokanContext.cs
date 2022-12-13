using Dokan.Domain.UsersAndRoles;
using Dokan.Domain.Website;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Core.Database
{
    public class DokanContext : IdentityDbContext<User>
    {
        #region Constructor

        public DokanContext()
            : base("DokanConnection", throwIfV1Schema: false)
        {
        }

        #endregion


        #region Methods

        public static DokanContext Create()
        {
            return new DokanContext();
        }

        #endregion


        #region Properties (Website tables)

        public DbSet<UserInformation> UserInformation { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<Log> Logs { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<KeyValueContent> KeyValueContents { get; set; }
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<TaxCategory> TaxCategories { get; set; }
        public DbSet<DiscountCategory> DiscountCategories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }


        #endregion
    }
}
