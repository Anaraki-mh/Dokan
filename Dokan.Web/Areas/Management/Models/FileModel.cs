using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace Dokan.Web.Areas.Management.Models
{
    public class FileModel
    {
        #region Constructor

        public FileModel()
        {

        }

        #endregion


        #region Properties
        public int Index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create date")]
        public DateTime CreateDateTime { get; set; }


        [Display(Name = "Title")]
        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(30, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Display(Name = "File Type")]
        [Required(ErrorMessage = "{0} can not be empty")]
        public FileType FileType { get; set; }

        #endregion


        #region Conversion Helpers

        public static FileModel EntityToModel(in File entity, int index = 0)
        {
            var model = new FileModel()
            {
                Id = entity.Id,
                Index = index,
                Title = entity.Title,
                FileType = entity.FileType,

                CreateDateTime = entity.CreateDateTime,
            };

            return model;
        }

        public static File ModelToEntity(in FileModel model)
        {
            var entity = new File()
            {
                Id = model.Id,
                Title = model.Title,
                FileType = model.FileType,
                CreateDateTime = model.CreateDateTime,
            };

            return entity;
        }


        #endregion
    }
}