using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Enums
{
    public enum PaymentState
    {
        Paid = 0,
        Pending = 1,
        PendingRefund = 2,
        Refunded = 3,
        Canceled = -1,
    }
}
