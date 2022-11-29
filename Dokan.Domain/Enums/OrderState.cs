﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Enums
{
    public enum OrderState
    {
        Completed = 0,
        Pending = 1,
        Placed = 2,
        Processing = 3,
        Canceled = -1,
    }
}
