using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Dokan.Web.Areas.Management.Models
{
    public class OrderItemModel
    {
        #region Constructor

        public OrderItemModel()
        {

        }

        #endregion

        #region Properties

        public int Id { get; set; }

        [Display(Name = "Price")]
        public double Price { get; set; }

        [Display(Name = "Discount")]
        public int Discount { get; set; }

        [Display(Name = "Tax")]
        public int Tax { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Total")]
        public double Total { get; set; }

        [Display(Name = "Price")]
        public string ProductTitle { get; set; }

        public int ProductId { get; set; }

        #endregion

        #region Relations

        public int CartId { get; set; }
        public virtual OrderModel CartModel { get; set; }

        #endregion

    }
}