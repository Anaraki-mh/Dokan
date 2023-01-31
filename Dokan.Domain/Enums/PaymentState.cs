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
        Refunded = 2,
        Canceled = -1,
    }
}
