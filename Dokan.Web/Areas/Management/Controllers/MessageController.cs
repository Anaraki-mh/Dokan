using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using Dokan.WebEssentials;
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
    public class MessageController : ManagementBaseController
    {
        #region Properties and fields

        private IMessageService _messageService { get; }
        private ILogService _logService { get; }
        private IEmailService _emailService { get; }

        private List<Message> _allEntities { get; set; }
        private MessageModel _model;
        private Message _entity;

        #endregion


        #region Constructor

        public MessageController(IMessageService MessageService, ILogService logService, IEmailService emailService)
        {
            _messageService = MessageService;
            _logService = logService;
            _emailService = emailService;

            _allEntities = new List<Message>();
            _model = new MessageModel();
            _entity = new Message();
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
            List<MessageModel> convertedEntityList = new List<MessageModel>();

            _allEntities = await _messageService.ListAsync();

            List<Message> filteredList = _allEntities.Skip((page - 1) * numberOfResults).Take(numberOfResults).ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in filteredList)
            {
                _model = new MessageModel();
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
            _entity = await _messageService.FindByIdAsync(id);

            EntityToModel(_entity, ref _model);

            return PartialView("_Details", _model);
        }

        [HttpGet]
        public async Task<ActionResult> Reply(int id)
        {
            try
            {
                _entity = await _messageService.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            EmptyModel(ref _model);

            _model.Id = _entity.Id;
            _model.Email = _entity.Email;
            _model.Subject = "WEBSITENAME - In reply to the message you left us!";

            return View(_model);
        }

        [HttpPost]
        public async Task<ActionResult> Reply(MessageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Id == 0)
                return View("Error400");

            try
            {
                string emailBody = EmailTemplate.PrepareReply("Thank you for contacting us!", "", model.MessageBody, "");

                _emailService.SendEmail(model.Subject, emailBody, model.Email);

                await Log(LogType.ContentUpdate, "Reply", $"{model.Id}_ {model.Subject}");
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return View("Error400");
            }

            return RedirectToAction("Index", "Message");
        }

        [HttpGet]
        public ActionResult Trash()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TrashList()
        {
            List<MessageModel> convertedEntityList = new List<MessageModel>();
            List<Message> removedEntityList = await _messageService.ListOfRemovedAsync();

            int index = 1;

            foreach (var entity in removedEntityList)
            {
                _model = new MessageModel();
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
                await _messageService.DeleteAsync(id);

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
                await _messageService.RemoveAsync(id);
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
                await _messageService.RestoreAsync(id);
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
                await _messageService.DeleteRangeAsync(await _messageService.ListOfRemovedAsync());
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

        private void EntityToModel(Message entity, ref MessageModel model)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Name = entity.Name;
            model.Email = entity.Email;
            model.Subject = entity.Subject;
            model.MessageBody = entity.MessageBody;
            model.CreateDateTime = entity.CreateDateTime;
        }

        private void EntityToModel(Message entity, ref MessageModel model, int index)
        {
            EmptyModel(ref model);

            model.Id = entity.Id;
            model.Index = index;
            model.Name = entity.Name;
            model.Email = entity.Email;
            model.Subject = entity.Subject;
            model.MessageBody = entity.MessageBody;
            model.CreateDateTime = entity.CreateDateTime;
        }

        private void ModelToEntity(MessageModel model, ref Message entity)
        {
            EmptyEntity(ref entity);

            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.Email = model.Email;
            entity.Subject = model.Subject;
            entity.MessageBody = model.MessageBody;
            entity.CreateDateTime = model.CreateDateTime;
        }

        private void EmptyEntity(ref Message entity)
        {
            entity.Id = 0;
            entity.Name = "";
            entity.Email = "";
            entity.Subject = "";
            entity.MessageBody = "";
            entity.CreateDateTime = DateTime.UtcNow;
        }

        private void EmptyModel(ref MessageModel model)
        {
            model.Id = 0;
            model.Index = 0;
            model.Name = "";
            model.Subject = "";
            model.MessageBody = "";
            model.Email = "";
            model.CreateDateTime = DateTime.UtcNow;
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