using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class OrderItem
    {
        #region Constructor

        public OrderItem()
        {

        }

        #endregion


        #region Properties

        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
        public int Discount { get; set; }
        public int Tax { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }

        #endregion


        #region Relations

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        #endregion
    }
}
