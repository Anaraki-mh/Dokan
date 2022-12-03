using Dokan.Domain.Enums;
using Dokan.Domain.UsersAndRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class Order
    {
        #region Constructor

        public Order()
        {
            OrderState = OrderState.Pending;
            PaymentState = PaymentState.Pending;
            OrderItems = new List<OrderItem>();
            CreateDateTime = DateTime.UtcNow;
        }

        #endregion


        #region Properties

        [Key]
        public int Id { get; set; }

        public DateTime CreateDateTime { get; set; }

        [MaxLength(50)]
        public string TrackingCode { get; set; }

        [MaxLength(50)]
        public string ProfilePicture { get; set; }

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

        public OrderState OrderState { get; set; }

        public PaymentState PaymentState { get; set; }

        #endregion


        #region Relations

        [MaxLength(100)]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; }

        #endregion
    }
}
