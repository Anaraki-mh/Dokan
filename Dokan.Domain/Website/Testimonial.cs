using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class Testimonial : BaseEntity
    {
        #region Properties

        [MaxLength(50)]
        public string FullName { get; set; }

        [MaxLength(35)]
        public string Position { get; set; }

        [MaxLength(300)]
        public string Content { get; set; }

        #endregion


        #region 

        public int ImageId { get; set; }
        public virtual File Image { get; set; }

        #endregion
    }
}
