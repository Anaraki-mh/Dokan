using Dokan.Domain.UsersAndRoles;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Core.Database
{
    public class DokanContext : IdentityDbContext<User>
    {
        public DokanContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static DokanContext Create()
        {
            return new DokanContext();
        }
    }
}
