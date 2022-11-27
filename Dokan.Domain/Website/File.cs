using Dokan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
            ProductImages = new List<Product>();
            ProductMainImage = new List<Product>();
            BlogPosts = new List<BlogPost>();
            Testimonials = new List<Testimonial>();
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

        public virtual List<Testimonial> Testimonials { get; set; }

        [InverseProperty("Images")]
        public virtual List<Product> ProductImages { get; set; }

        [InverseProperty("MainImage")]
        public virtual List<Product> ProductMainImage { get; set; }

        public virtual List<BlogPost> BlogPosts { get; set; }
        #endregion
    }
}
