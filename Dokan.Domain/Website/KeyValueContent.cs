using Dokan.Domain.BaseData;
using Dokan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dokan.Domain.Website
{
    public class KeyValueContent : BaseEntity
    {
        [MaxLength(50)]
        public string ContentKey { get; set; }

        [AllowHtml]
        [MaxLength(1000)]
        public string ContentValue { get; set; }

        [MaxLength(75)]
        public string Description { get; set; }
    }
}
