﻿using Dokan.Domain.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Domain.Website
{
    public class Carousel : BaseEntity
    {
        #region Properties

        public bool Display { get; set; }

        [MaxLength(75)]
        public string Title { get; set; }

        [MaxLength(80)]
        public string Description { get; set; }

        [MaxLength(30)]
        public string ButtonOne { get; set; }

        [MaxLength(40)]
        public string LinkOne { get; set; }

        [MaxLength(30)]
        public string ButtonTwo { get; set; }

        [MaxLength(40)]
        public string LinkTwo { get; set; }

        #endregion


        #region Relations

        public int ImageId { get; set; }
        public virtual File Image { get; set; }

        #endregion
    }
}
