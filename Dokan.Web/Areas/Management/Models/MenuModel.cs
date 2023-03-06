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
    public class MenuModel
    {
        #region Constructor

        public MenuModel()
        {
            ParentDropdown = new List<SelectListItem>();
        }

        #endregion


        #region Properties
        public int Index { get; set; }

        public int Id { get; set; }

        [Display(Name = "Create/Update date")]
        public DateTime UpdateDateTime { get; set; }


        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(45, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "{0} can not be empty")]
        [MaxLength(75, ErrorMessage = "{0} can not be longer than {1} characters")]
        public string Link { get; set; }

        [Display(Name = "Display")]
        public bool IsDisplayed { get; set; }

        [Display(Name = "Display priority")]
        public int? Priority { get; set; }


        [Display(Name = "Parent")]
        public int? ParentId { get; set; }

        public string ParentTitle { get; set; }

        public List<SelectListItem> ParentDropdown { get; set; }

        #endregion


        #region Conversion Helpers


        public static MenuModel EntityToModel(in Menu entity, int index = 0)
        {
            var mode = new MenuModel()
            {
                Id = entity.Id,
                Index = index,
                Title = entity.Title,
                IsDisplayed = entity.IsDisplayed,
                Link = entity.Link,
                Priority = entity.Priority,
                ParentId = entity.ParentId,
                ParentTitle = entity.Parent?.Title ?? " - ",
                UpdateDateTime = entity.UpdateDateTime,
            };

            return mode;
        }

        public static Menu ModelToEntity(in MenuModel model)
        {
            var entity = new Menu()
            {
                Id = model.Id,
                Title = model.Title,
                IsDisplayed = model.IsDisplayed,
                Link = model.Link,
                Priority = model.Priority ?? 0,
                ParentId = model.ParentId,
            };

            return entity;
        }

        #endregion


        #region Preparation Helpers

        public static void PrepareDropdown(ref MenuModel model, List<Menu> dropdownItemsList)
        {
            model.ParentDropdown.Clear();

            model.ParentDropdown.Add(new SelectListItem()
            {
                Text = "Select an item...",
                Value = "",
            });

            int modelId = model.Id;
            dropdownItemsList.Remove(dropdownItemsList.FirstOrDefault(x => x.Id == modelId));

            foreach (var entity in dropdownItemsList)
            {
                model.ParentDropdown.Add(new SelectListItem()
                {
                    Text = entity.Title,
                    Value = entity.Id.ToString(),
                });
            }
        }


        #endregion
    }
}