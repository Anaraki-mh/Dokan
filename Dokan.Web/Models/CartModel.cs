using Dokan.Domain.Enums;
using Dokan.Domain.UsersAndRoles;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dokan.Web.Models
{
    public class CartModel
    {

        #region Constructor

        public CartModel()
        {
            OrderState = OrderState.Pending;
            PaymentState = PaymentState.Pending;
            CartItems = new List<CartItemModel>();
            CreateDateTime = DateTime.UtcNow;
        }

        #endregion


        #region Properties

        public int Id { get; set; }

        public DateTime CreateDateTime { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(25)]
        public string Password { get; set; }
        [MaxLength(25)]
        public string ConfirmPassword { get; set; }

        [MaxLength(50)]
        public string TrackingCode { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(120)]
        public string Address { get; set; }

        [MaxLength(12)]
        public string ZipCode { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public double ShippingCost { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        public PaymentState PaymentState { get; set; }

        public OrderState OrderState { get; set; }

        #endregion


        #region Relations

        [MaxLength(100)]
        public string UserId { get; set; }

        public int? CouponId { get; set; }
        public virtual Coupon Coupon { get; set; }

        public virtual List<CartItemModel> CartItems { get; set; }

        #endregion
    }
}