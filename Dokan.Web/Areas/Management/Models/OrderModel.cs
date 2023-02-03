using Dokan.Domain.Enums;
using Dokan.Domain.UsersAndRoles;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dokan.Web.Areas.Management.Models
{
    public class OrderModel
    {

        #region Constructor

        public OrderModel()
        {
            OrderState = OrderState.Pending;
            PaymentState = PaymentState.Pending;
            CartItems = new List<OrderItemModel>();
            CreateDateTime = DateTime.UtcNow;
        }

        #endregion


        #region Properties

        public int Id { get; set; }
        public int Index { get; set; }

        [Display(Name = "Create Date")]
        public DateTime CreateDateTime { get; set; }

        [MaxLength(50)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(50)]
        [Display(Name = "Payment ID")]
        public string TrackingCode { get; set; }

        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [MaxLength(50)]
        [Display(Name = "State")]
        public string State { get; set; }

        [MaxLength(50)]
        [Display(Name = "City")]
        public string City { get; set; }

        [MaxLength(120)]
        [Display(Name = "Street Address")]
        public string Address { get; set; }

        [MaxLength(12)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [MaxLength(15)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Shipping Cost")]
        public double ShippingCost { get; set; }

        [Display(Name = "Delivery Method")]
        public DeliveryMethod DeliveryMethod { get; set; }

        [Display(Name = "Payment State")]
        public PaymentState PaymentState { get; set; }

        [Display(Name = "Order State")]
        public OrderState OrderState { get; set; }

        #endregion


        #region Relations

        [MaxLength(100)]
        public string UserId { get; set; }

        public int? CouponId { get; set; }
        public virtual Coupon Coupon { get; set; }

        public virtual List<OrderItemModel> CartItems { get; set; }

        #endregion
    }
}