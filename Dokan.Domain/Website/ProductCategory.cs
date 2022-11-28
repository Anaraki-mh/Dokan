﻿using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class ProductCategory : BaseEntity
    {
        #region Constructor

        public ProductCategory()
        {
            Products = new List<Product>();
        }

        #endregion


        #region Properties

        [MaxLength(40)]
        public string Title { get; set; }

        public int Priority { get; set; }

        #endregion


        #region Relations

        public virtual List<Product> Products { get; set; }

        public int? ParentId { get; set; }
        public virtual ProductCategory Parent { get; set; }

        public int? PricingRuleId { get; set; }
        public virtual ProductCategoryPricingRule PricingRule { get; set; }

        #endregion
    }
}
