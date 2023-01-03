using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
    public class CarouselController : Controller
    {
        #region Properties and fields

        private ICarouselService _carouselService { get; }
        private ILogService _logService { get; }

        private List<Carousel> _allEntities { get; set; }
        private CarouselModel _model;
        private Carousel _entity;

        #endregion


        #region Constructor

        public CarouselController(ICarouselService CarouselService, ILogService logService)
        {
            _carouselService = CarouselService;
            _logService = logService;

            _allEntities = new List<Carousel>();
            _model = new CarouselModel();
            _entity = new Carousel();
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
            List<CarouselModel> convertedEntityList = new List<CarouselModel>();

            _allEntities = await _carouselService.ListAsync();

            List<Carousel> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new CarouselModel();
                EntityToModel(entity, ref _model, index);

                convertedEntityList.Add(_model);

                index++;
            }

            ViewBag.NumberOfPages = Math.Ceiling((decimal)_allEntities.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            _entity = await _carouselService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            EmptyModel(ref _model);

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CarouselModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.CreateDateTime = DateTime.UtcNow;
                _entity.UpdateDateTime = DateTime.UtcNow;

                await _carouselService.CreateAsync(_entity);

                await Log(LogType.ContentAdd, "Create", $"{_entity.Id}_ {_entity.Title}");
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
            try
            {
                _entity = await _carouselService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            EntityToModel(_entity, ref _model);

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(CarouselModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.UpdateDateTime = DateTime.UtcNow;

                await _carouselService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.Title}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "Carousel");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<CarouselModel> convertedEntityList = new List<CarouselModel>();
            List<Carousel> removedEntityList = await _carouselService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new CarouselModel();
                EntityToModel(entity, ref _model, index);

                convertedEntityList.Add(_model);

                index++;
            }


            return PartialView("_TrashList", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _carouselService.DeleteAsync(id);

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
                await _carouselService.RemoveAsync(id);
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
                await _carouselService.RestoreAsync(id);
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
                await _carouselService.DeleteRangeAsync(await _carouselService.ListOfRemovedAsync());
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


        #region Conversion Methods

        private void EntityToModel(Carousel entity, ref CarouselModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Description = entity.Description;
            model.Priority= entity.Priority;
            model.IsDisplayed = entity.IsDisplayed;
            model.Image= entity.Image;
            model.ButtonOne= entity.ButtonOne;
            model.ButtonTwo= entity.ButtonTwo;
            model.LinkOne= entity.LinkOne;
            model.LinkTwo= entity.LinkTwo;

            model.TitleColor= entity.TitleColor;
            model.DescriptionColor= entity.DescriptionColor;
            model.ButtonOneBgColor= entity.ButtonOneBgColor;
            model.ButtonTwoBgColor = entity.ButtonTwoBgColor;
            model.ButtonOneFgColor= entity.ButtonOneFgColor;
            model.ButtonTwoFgColor = entity.ButtonTwoFgColor;

            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(Carousel entity, ref CarouselModel model, int index)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Index = index;
            model.Title = entity.Title;
            model.Description = entity.Description;
            model.IsDisplayed = entity.IsDisplayed;
            model.Priority = entity.Priority;
            model.Image = entity.Image;
            model.ButtonOne = entity.ButtonOne;
            model.ButtonTwo = entity.ButtonTwo;
            model.LinkOne = entity.LinkOne;
            model.LinkTwo = entity.LinkTwo;

            model.TitleColor = entity.TitleColor;
            model.DescriptionColor = entity.DescriptionColor;
            model.ButtonOneBgColor = entity.ButtonOneBgColor;
            model.ButtonTwoBgColor = entity.ButtonTwoBgColor;
            model.ButtonOneFgColor = entity.ButtonOneFgColor;
            model.ButtonTwoFgColor = entity.ButtonTwoFgColor;
            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void ModelToEntity(CarouselModel model, ref Carousel entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.IsDisplayed = model.IsDisplayed;
            entity.Priority= model.Priority;
            entity.Image = model.Image;
            entity.ButtonOne = model.ButtonOne;
            entity.ButtonTwo = model.ButtonTwo;
            entity.LinkOne = model.LinkOne;
            entity.LinkTwo = model.LinkTwo;

            entity.TitleColor = model.TitleColor;
            entity.DescriptionColor = model.DescriptionColor;
            entity.ButtonOneBgColor = model.ButtonOneBgColor;
            entity.ButtonTwoBgColor = model.ButtonTwoBgColor;
            entity.ButtonOneFgColor = model.ButtonOneFgColor;
            entity.ButtonTwoFgColor = model.ButtonTwoFgColor;
            entity.UpdateDateTime = DateTime.UtcNow;

        }

        private void EmptyEntity(ref Carousel entity)
        {
            entity.Id = 0;
            entity.Title = "";
            entity.Description = "";
            entity.IsDisplayed = false;
            entity.Priority = 0;
            entity.Image = "";
            entity.ButtonOne = "";
            entity.ButtonTwo = "";
            entity.LinkOne = "";
            entity.LinkTwo = "";

            entity.TitleColor = "#ffffff";
            entity.DescriptionColor = "#ffffff";
            entity.ButtonOneBgColor = "#000000";
            entity.ButtonTwoBgColor = "#000000";
            entity.ButtonOneFgColor = "#ffffff";
            entity.ButtonTwoFgColor = "#ffffff";
            entity.UpdateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref CarouselModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.Title = "";
            model.Description = "";
            model.IsDisplayed = false;
            model.Priority = 0;
            model.Image = "";
            model.ButtonOne = "";
            model.ButtonTwo = "";
            model.LinkOne = "";
            model.LinkTwo = "";

            model.TitleColor = "#ffffff";
            model.DescriptionColor = "#ffffff";
            model.ButtonOneBgColor = "#000000";
            model.ButtonTwoBgColor = "#000000";
            model.ButtonOneFgColor = "#ffffff";
            model.ButtonTwoFgColor = "#ffffff";
            model.UpdateDateTime = DateTime.UtcNow;
        }

        #endregion


        #region Log and Preperation Methods

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