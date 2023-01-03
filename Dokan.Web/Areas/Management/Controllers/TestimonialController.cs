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
    public class TestimonialController : Controller
    {
        #region Properties and fields

        private ITestimonialService _testimonialService { get; }
        private ILogService _logService { get; }

        private List<Testimonial> _allEntities { get; set; }
        private TestimonialModel _model;
        private Testimonial _entity;

        #endregion


        #region Constructor

        public TestimonialController(ITestimonialService TestimonialService, ILogService logService)
        {
            _testimonialService = TestimonialService;
            _logService = logService;

            _allEntities = new List<Testimonial>();
            _model = new TestimonialModel();
            _entity = new Testimonial();
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
            List<TestimonialModel> convertedEntityList = new List<TestimonialModel>();

            _allEntities = await _testimonialService.ListAsync();

            List<Testimonial> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new TestimonialModel();
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
            _entity = await _testimonialService.FindByIdAsync(id);

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
        public async Task<ActionResult> Create(TestimonialModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.CreateDateTime = DateTime.UtcNow;
                _entity.UpdateDateTime = DateTime.UtcNow;

                await _testimonialService.CreateAsync(_entity);

                await Log(LogType.ContentAdd, "Create", $"{_entity.Id}_ {_entity.FullName}");
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
                _entity = await _testimonialService.FindByIdAsync(id);
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
        public async Task<ActionResult> Update(TestimonialModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ModelToEntity(model, ref _entity);

                _entity.UpdateDateTime = DateTime.UtcNow;

                await _testimonialService.UpdateAsync(_entity);

                await Log(LogType.ContentUpdate, "Update", $"{_entity.Id}_ {_entity.FullName}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "Testimonial");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<TestimonialModel> convertedEntityList = new List<TestimonialModel>();
            List<Testimonial> removedEntityList = await _testimonialService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new TestimonialModel();
                EntityToModel(entity, ref _model, index);

                convertedEntityList.Add(_model);

                index++;
            }


            return PartialView("_TrashList" ,convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _testimonialService.DeleteAsync(id);

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
                await _testimonialService.RemoveAsync(id);
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
                await _testimonialService.RestoreAsync(id);
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
                await _testimonialService.DeleteRangeAsync(await _testimonialService.ListOfRemovedAsync());
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

        private void EntityToModel(Testimonial entity, ref TestimonialModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.FullName = entity.FullName;
            model.Position = entity.Position;
            model.Content = entity.Content;
            model.Image = entity.Content;

            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void EntityToModel(Testimonial entity, ref TestimonialModel model, int index)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.index = index;
            model.FullName = entity.FullName;
            model.Position = entity.Position;
            model.Content = entity.Content;
            model.Image = entity.Content;

            model.UpdateDateTime = entity.UpdateDateTime;
        }

        private void ModelToEntity(TestimonialModel model, ref Testimonial entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.FullName = model.FullName;
            entity.Position = model.Position;
            entity.Content = model.Content;
            entity.Image = model.Content;
        }

        private void EmptyEntity(ref Testimonial entity)
        {
            entity.Id = 0;
            entity.FullName = "";
            entity.Position = "";
            entity.Content = "";
            entity.Image = "";
            entity.UpdateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref TestimonialModel model)
        {
            model.Id = 0;
            model.index = 0;
            model.FullName = "";
            model.Position = "";
            model.Content = "";
            model.Image = "";
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