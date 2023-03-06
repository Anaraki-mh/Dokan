using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class MenuController : ManagementBaseController
    {
        #region Properties and fields

        private IMenuService _menuService { get; }
        private ILogService _logService { get; }

        #endregion


        #region Constructor

        public MenuController(IMenuService MenuService, ILogService logService)
        {
            _menuService = MenuService;
            _logService = logService;
        }

        #endregion


        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> List(int page = 1, int numberOfResults = 5)
        {
            List<MenuModel> convertedEntityList = new List<MenuModel>();

            var allEntities = await _menuService.ListAsync();

            List<Menu> filteredList = allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                var model = MenuModel.EntityToModel(in entity, index);

                convertedEntityList.Add(model);

                index++;
            }

            ViewBag.NumberOfPages = Math.Ceiling((decimal)allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var entity = await _menuService.FindByIdAsync(id);
            var model = MenuModel.EntityToModel(in entity);

            return PartialView("_Details", model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new MenuModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(MenuModel model)
        {
            if (!ModelState.IsValid)
            {
                MenuModel.PrepareDropdown(ref model, await _menuService.ListAsync());
                return View(model);
            }

            try
            {
                var entity = MenuModel.ModelToEntity(in model);

                entity.CreateDateTime = DateTime.UtcNow;
                entity.UpdateDateTime = DateTime.UtcNow;

                await _menuService.CreateAsync(entity);

                await Log(LogType.ContentAdd, "Create", $"{entity.Id}_ {entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var entity = new Menu();
            try
            {
                entity = await _menuService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            var model = MenuModel.EntityToModel(in entity);

            MenuModel.PrepareDropdown(ref model, await _menuService.ListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(MenuModel model)
        {
            if (!ModelState.IsValid)
            {
                MenuModel.PrepareDropdown(ref model, await _menuService.ListAsync());
                return View(model);
            }

            try
            {
                var entity = MenuModel.ModelToEntity(in model);

                entity.UpdateDateTime = DateTime.UtcNow;

                await _menuService.UpdateAsync(entity);

                await Log(LogType.ContentUpdate, "Update", $"{entity.Id}_ {entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "Menu");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<MenuModel> convertedEntityList = new List<MenuModel>();
            List<Menu> removedEntityList = await _menuService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                var model = MenuModel.EntityToModel(in entity, index);
                convertedEntityList.Add(model);

                index++;
            }


            return PartialView("_TrashList" ,convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _menuService.DeleteAsync(id);

                await Log(LogType.ContentDelete, "Delete", $"{id}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> Remove(int id)
        {
            try
            {
                await _menuService.RemoveAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> Restore(int id)
        {
            try
            {
                await _menuService.RestoreAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteAllTrash()
        {
            try
            {
                await _menuService.DeleteRangeAsync(await _menuService.ListOfRemovedAsync());
                await Log(LogType.ContentAdd, "DeleteAllTrash", $"Deleted all items in the trash");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #endregion


        #region Log Methods

        private async Task LogError(Exception ex)
        {
            await _logService.CreateAsync(new Log()
            {
                LogType = LogType.Error,
                Controller = ex.Source,
                Method = ex.TargetSite.Name,
                Description = ex.Message,
                AdditionalInfo = ex.InnerException.ToString(),
                Code = ex.HResult.ToString(),
            });
        }

        private async Task Log(LogType logType, string method, string description)
        {
            await _logService.CreateAsync(new Log()
            {
                LogType = logType,
                Controller = this.GetType().Name,
                Method = method,
                Description = $"{logType} _ {description}",
            });
        }

        #endregion
    }
}