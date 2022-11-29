using Dokan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class Log
    {
        #region Constructor

        public Log()
        {

        }

        #endregion


        #region Properties

        [Key]
        public int Id { get; set; }

        public LogType LogType { get; set; }

        [MaxLength(100)]
        public string Controller { get; set; }

        [MaxLength(100)]
        public string Method { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(500)]
        public string AdditionalInfo { get; set; }

        #endregion
    }
}
