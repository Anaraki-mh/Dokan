using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class ProductCategoryPricingRule : BaseEntity
    {
        #region Constructor

        public ProductCategoryPricingRule()
        {
            ProductCategories = new List<ProductCategory>();
        }

        #endregion


        #region Properties

        public string Title { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiryDateTime { get; set; }

        public int Tax { get; set; }
        public int Discount { get; set; }
        public int MultiplyPriceBy { get; set; }

        #endregion


        #region Relations

        public virtual List<ProductCategory> ProductCategories { get; set; }

        #endregion
    }
}
