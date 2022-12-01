using Dokan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.UsersAndRoles
{
    public class UserInformation
    {
        #region Constructor 

        public UserInformation()
        {

        }

        #endregion


        #region Properties

        [Key]
        public int Id { get; set; }

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

        #endregion


        #region Relations

        public string UserId { get; set; }

        [Required]
        public virtual User User { get; set; }

        #endregion
    }
}
