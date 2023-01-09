using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Dokan.WebEssentials
{
    public static class SEO
    {
        private static List<char> UnwiseOrReservedChars = new List<char> 
        {
            ' ' ,'{' , '}' , '|' , '\\' , '^' , '[' , ']' , '`', '<', '>', ';' , '/' , '?' , ':' , '@' , '&' , '=' , '+' , '$' , ','
        };

        public static string CreateSeoFriendlyUrlTitle(string title)
        {
            foreach (var character in UnwiseOrReservedChars)
            {
                title = title.Replace(character, '-');
            }
            return title;
        }
    }
}
