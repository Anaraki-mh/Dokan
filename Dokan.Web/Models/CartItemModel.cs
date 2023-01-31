using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dokan.Web.Models
{
    public class CartItemModel
    {
        #region Constructor

        public CartItemModel()
        {

        }

        #endregion

        #region Properties

        public int Id { get; set; }
        public double Price { get; set; }
        public int Discount { get; set; }
        public int Tax { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }

        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductImage { get; set; }

        #endregion

        #region Relations

        public int CartId { get; set; }
        public virtual CartModel CartModel { get; set; }

        #endregion

    }
}