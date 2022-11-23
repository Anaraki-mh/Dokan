using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            Images = new List<File>();
            OrderItems = new List<OrderItem>();
        }

        #endregion


        #region Properties

        [MaxLength(40)]
        public string Title { get; set; }

        [MaxLength(40)]
        public string ShortDescription { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        [MaxLength(40)]
        public string MainImage { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }

        #endregion


        #region Relations

        public virtual List<ProductComment> ProductComments { get; set; }

        public int ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }

        public virtual List<File> Images { get; set; }
        public virtual List<OrderItem> OrderItems { get; set; }

        #endregion
    }
}
