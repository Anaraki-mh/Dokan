using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class Product : BaseEntity
    {

        #region Constructor

        public Product()
        {
            ProductComments = new List<ProductComment>();
            OrderItems = new List<OrderItem>();
        }

        #endregion


        #region Properties

        [MaxLength(40)]
        public string Title { get; set; }

        public int MainImageId { get; set; }

        [MaxLength(40)]
        public string ShortDescription { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public double Price { get; set; }
        public int Stock { get; set; }

        [MaxLength(30)]
        public string Image1 { get; set; }

        [MaxLength(30)]
        public string Image2 { get; set; }

        [MaxLength(30)]
        public string Image3 { get; set; }

        [MaxLength(30)]
        public string Image4 { get; set; }

        [MaxLength(30)]
        public string Image5 { get; set; }


        #endregion


        #region Relations

        public virtual List<ProductComment> ProductComments { get; set; }

        public int ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; }

        public int? DiscountCategoryId { get; set; }
        public virtual DiscountCategory DiscountCategory { get; set; }

        #endregion
    }
}
