using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class Menu : BaseEntity
    {

        #region Properties

        [MaxLength(45)]
        public string Title { get; set; }

        [MaxLength(75)]
        public string Link { get; set; }

        #endregion


        #region Relations

        public int? ParentId { get; set; }
        public virtual Menu Parent { get; set; }

        #endregion
    }
}
