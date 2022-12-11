using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.BaseData
{
    public class BaseComment : BaseEntity
    {
        [MaxLength(40)]
        public string Title { get; set; }

        [MaxLength(300)]
        public string Body { get; set; }

        public bool IsApproved { get; set; }

        public int Rating { get; set; }
    }
}
