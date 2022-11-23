using Dokan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class File
    {
        #region Constructor

        public File()
        {
            CreateDateTime = DateTime.UtcNow;
            Products = new List<Product>();
        }

        #endregion


        #region Properties

        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        public DateTime CreateDateTime { get; set; }

        public FileType FileType { get; set; }

        #endregion


        #region Relations

        public virtual List<Product> Products { get; set; }

        #endregion
    }
}
