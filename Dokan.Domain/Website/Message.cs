using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class Message : BaseEntity
    {
        #region Constructor

        public Message()
        {

        }

        #endregion


        #region Properties

        [MaxLength(75)]
        public string Name { get; set; }

        [MaxLength(75)]
        public string Email { get; set; }

        [MaxLength(75)]
        public string Subject { get; set; }

        [MaxLength(400)]
        public string MessageBody { get; set; }

        #endregion
    }
}
