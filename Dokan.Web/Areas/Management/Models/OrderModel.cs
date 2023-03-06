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
            OrderItems = new List<OrderItemModel>();
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
        [Display(Name = "Zip / Post Code")]
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

        public virtual List<OrderItemModel> OrderItems { get; set; }

        #endregion


        #region Conversion Helpers

        public static OrderModel OrderEntityToModel(in Order entity, int index = 0)
        {
            var model = new OrderModel()
            {
                Id = entity.Id,
                Index = index,
                CreateDateTime = entity.CreateDateTime,
                TrackingCode = entity.TrackingCode,
                FirstName = entity.User?.UserInformation?.FirstName ?? "",
                LastName = entity.User?.UserInformation?.LastName ?? "",
                Country = entity.User?.UserInformation?.Country ?? "",
                State = entity.User?.UserInformation?.State ?? "",
                City = entity.User?.UserInformation?.City ?? "",
                Address = entity.User?.UserInformation?.Address ?? "",
                ZipCode = entity.User?.UserInformation?.ZipCode ?? "",
                PhoneNumber = entity.User?.UserInformation?.PhoneNumber ?? "",
                ShippingCost = entity.ShippingCost,
                DeliveryMethod = entity.DeliveryMethod,
                PaymentState = entity.PaymentState,
                OrderState = entity.OrderState,
                UserId = entity.UserId,
                Coupon = entity.Coupon,
                CouponId = entity.CouponId,
            };

            foreach (var item in entity.OrderItems)
            {
                model.OrderItems.Add(new OrderItemModel
                {
                    Id = item.Id,
                    CartId = model.Id,
                    ProductId = item.ProductId,
                    ProductTitle = item.Product.Title,
                    Discount = item.Discount,
                    Tax = item.Tax,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Total = item.Total,
                });
            }

            return model;
        }

        #endregion
    }
}