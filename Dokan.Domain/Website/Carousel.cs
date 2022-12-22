using Dokan.Domain.BaseData;
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

        public bool IsDisplayed { get; set; }

        [MaxLength(30)]
        public string Image { get; set; }

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


        [MaxLength(8)]
        public string TitleColor { get; set; }

        [MaxLength(8)]
        public string DescriptionColor { get; set; }

        [MaxLength(8)]
        public string ButtonOneBgColor { get; set; }

        [MaxLength(8)]
        public string ButtonOneFgColor { get; set; }

        [MaxLength(8)]
        public string ButtonTwoBgColor { get; set; }

        [MaxLength(8)]
        public string ButtonTwoFgColor { get; set; }

        #endregion

    }
}
