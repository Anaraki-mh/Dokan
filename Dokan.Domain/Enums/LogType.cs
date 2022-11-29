using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Enums
{
    public enum LogType
    {
        Error = -1,
        Process = 0,
        ContentAdd = 10,
        ContentUpdate = 11,
        ContentDelete = 12,
    }
}
