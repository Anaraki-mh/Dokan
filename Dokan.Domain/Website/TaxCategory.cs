using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class TaxCategory : BaseEntity
    {
        #region Constructor

        public TaxCategory()
        {
            ProductCategories = new List<ProductCategory>();
        }

        #endregion


        #region Properties

        [MaxLength(75)]
        public string Title { get; set; }

        public int Tax { get; set; }

        #endregion


        #region Relations

        public virtual List<ProductCategory> ProductCategories { get; set; }

        #endregion
    }
}
