using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Areas.Management.Models;
using Dokan.Web.Models;
using Dokan.WebEssentials;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class OrderController : ManagementBaseController
    {
        #region Fields and Properties

        private IOrderService _orderService { get; }
        private IEmailService _emailService { get; }

        #endregion


        #region Constructor

        public OrderController(IOrderService orderService, IEmailService emailService)
        {
            _orderService = orderService;
            _emailService = emailService;
        }

        #endregion


        #region Methods

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> List(int page = 1, int numberOfResults = 5)
        {
            var convertedEntityList = new List<OrderModel>();

            var orderEntityList = await _orderService.ListAsync();

            orderEntityList = orderEntityList.Where(x => x.OrderState != OrderState.Pending)
                .Skip((page - 1) * numberOfResults)
                .Take(numberOfResults)
                .ToList();

            int index = (page - 1) * numberOfResults + 1;

            foreach (var entity in orderEntityList)
            {
                var orderModel = OrderModel.OrderEntityToModel(entity, index);

                convertedEntityList.Add(orderModel);

                index++;
            }

            convertedEntityList = convertedEntityList.OrderBy(x => x.CreateDateTime).ToList();

            ViewBag.NumberOfPages = Math.Ceiling((decimal)orderEntityList.Count / (decimal)numberOfResults);
            ViewBag.ActivePage = page;

            return PartialView("_List", convertedEntityList);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var entity = await _orderService.FindByIdAsync(id);

            if (entity is null)
                entity = new Order();

            var model = OrderModel.OrderEntityToModel(entity);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeOrderState(int id, string newState)
        {
            var entity = await _orderService.FindByIdAsync(id);

            if (entity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            switch (newState)
            {
                case "Processing":
                    entity.OrderState = OrderState.Processing;
                    break;
                case "Completed":
                    entity.OrderState = OrderState.Completed;
                    break;
                default:
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            await _orderService.UpdateAsync(entity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> CancelOrder(int id)
        {
            var entity = await _orderService.FindByIdAsync(id);

            if (entity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            entity.OrderState = OrderState.Canceled;
            await _orderService.UpdateAsync(entity);

            var emailBody = EmailTemplate.PrepareUpdate("Order Canceled!", $"Your order with the ID of #{entity.Id:00000} has been canceled", "");

            _emailService.SendEmail("Order Canceled And Refunded Successfully!", emailBody, entity.User.Email);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> CancelAndRefundOrder(int id, string reason)
        {
            var entity = await _orderService.FindByIdAsync(id);

            if (entity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            entity.PaymentState = PaymentState.PendingRefund;
            entity.OrderState = OrderState.Canceled;
            await _orderService.UpdateAsync(entity);

            var options = new RefundCreateOptions
            {
                PaymentIntent = entity.TrackingCode,
                Reason = reason
            };
            var service = new RefundService();
            service.Create(options);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<ActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.InputStream).ReadToEndAsync();
            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    WebConfigurationManager.AppSettings["StripeRefundWebhookSecret"]
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Faild to unmarshal event data. /n {e}");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //automate based on event type
            if (stripeEvent.Type == "charge.refunded")
            {
                var refund = stripeEvent.Data.Object as Refund;
                var paymentIndentId = refund.PaymentIntentId;

                // Update the status of the order
                var orderEntityList = await _orderService.ListAsync();
                var entity = orderEntityList.FirstOrDefault(x => x.TrackingCode == paymentIndentId);
                entity.OrderState = OrderState.Canceled;
                entity.PaymentState = PaymentState.Refunded;
                await _orderService.UpdateAsync(entity);

                // Send an email to the Admin to infrom them of the successful refund
                _emailService.SendEmail("Payment Refund Successful!", 
                    $"Order with ID of #{entity.Id:00000} and payment ID of {entity.TrackingCode} was successfully refunded." +
                    $"<br />Transaction number: {refund.ReceiptNumber}", 
                    WebConfigurationManager.AppSettings["AdminEmail"]);

                // Send the invoice to the user as an email
                var emailBody = EmailTemplate.PrepareUpdate("Order Canceled And Refunded!", $"Your order with the ID of #{entity.Id:00000} has been canceled" +
                    $"and your payment has been fully refunded with the transaction number being {refund.ReceiptNumber}. " +
                    $"<br /> <b>Reason for refund: </b> {refund.Reason}", "");

                _emailService.SendEmail("Order Canceled And Refunded Successfully!", emailBody, entity.User.Email);
            }

            //always have to return 200
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        #endregion
    }
}