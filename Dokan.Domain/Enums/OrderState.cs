using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Enums
{
    public enum OrderState
    {
        Incomplete = -1,
        Placed = 0,
        Processing = 1,
        Completed = 2,
    }
}
