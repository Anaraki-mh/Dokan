using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.WebEssentials
{
    public static class Typography
    {
        public static string TrimString(string stringData, int length)
        {
            if(stringData.Length > length - 3)
            {
                stringData = stringData.Substring(length - 3) + "...";
            }
            return stringData;
        }
    }
}
