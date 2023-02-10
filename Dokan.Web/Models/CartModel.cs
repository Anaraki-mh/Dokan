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

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(50, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [MaxLength(25, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Password { get; set; }

        [MaxLength(50)]
        public string TrackingCode { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(50, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(50, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string LastName { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(50, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Country { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(50, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string State { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(50, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string City { get; set; }

        [Display(Name = "Street Address")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(120, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Address { get; set; }

        [Display(Name = "Zip / Postcode")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(12, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string ZipCode { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(15, ErrorMessage = "{0} can not be longer than {1} characters")]
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