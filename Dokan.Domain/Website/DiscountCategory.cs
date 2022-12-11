using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class DiscountCategory : BaseEntity
    {
        #region Constructor

        public DiscountCategory()
        {
            Products = new List<Product>();
        }

        #endregion


        #region Properties

        [MaxLength(75)]
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiryDateTime { get; set; }
        public int Discount { get; set; }

        #endregion


        #region Relations

        public virtual List<Product> Products { get; set; }

        #endregion
    }
}
