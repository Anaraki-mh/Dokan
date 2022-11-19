using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.BaseData
{
    public class BaseEntity
    {

        #region Constructor

        public BaseEntity()
        {
            CreateDateTime = DateTime.UtcNow;
            UpdateDateTime = DateTime.UtcNow;
        }

        #endregion


        #region Properties

        [Key]
        public int Id { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        #endregion

    }
}
