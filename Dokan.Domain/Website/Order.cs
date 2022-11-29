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
            OrderState = OrderState.Incomplete;
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
