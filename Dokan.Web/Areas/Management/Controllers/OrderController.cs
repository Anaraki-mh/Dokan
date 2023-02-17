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

        private Order _orderEntity;
        private OrderModel _orderModel;
        private OrderItemModel _orderItemModel;

        #endregion


        #region Constructor

        public OrderController(IOrderService orderService, IEmailService emailService)
        {
            _orderService = orderService;
            _emailService = emailService;

            _orderEntity = new Order();
            _orderModel = new OrderModel();
            _orderItemModel = new OrderItemModel();
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
                _orderModel = new OrderModel();
                OrderEntityToModel(entity, ref _orderModel, index);

                convertedEntityList.Add(_orderModel);

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
            _orderEntity = await _orderService.FindByIdAsync(id);

            if (_orderEntity is null)
                _orderEntity = new Order();

            OrderEntityToModel(_orderEntity, ref _orderModel, 0);

            return View(_orderModel);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeOrderState(int id, string newState)
        {
            _orderEntity = await _orderService.FindByIdAsync(id);

            if (_orderEntity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            switch (newState)
            {
                case "Processing":
                    _orderEntity.OrderState = OrderState.Processing;
                    break;
                case "Completed":
                    _orderEntity.OrderState = OrderState.Completed;
                    break;
                default:
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            await _orderService.UpdateAsync(_orderEntity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> CancelOrder(int id)
        {
            _orderEntity = await _orderService.FindByIdAsync(id);

            if (_orderEntity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _orderEntity.OrderState = OrderState.Canceled;
            await _orderService.UpdateAsync(_orderEntity);

            var emailBody = EmailTemplate.PrepareUpdate("Order Canceled!", $"Your order with the ID of #{_orderEntity.Id:00000} has been canceled", "");

            _emailService.SendEmail("Order Canceled And Refunded Successfully!", emailBody, _orderEntity.User.Email);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> CancelAndRefundOrder(int id, string reason)
        {
            _orderEntity = await _orderService.FindByIdAsync(id);

            if (_orderEntity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _orderEntity.PaymentState = PaymentState.PendingRefund;
            _orderEntity.OrderState = OrderState.Canceled;
            await _orderService.UpdateAsync(_orderEntity);

            var options = new RefundCreateOptions
            {
                PaymentIntent = _orderEntity.TrackingCode,
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
                    WebConfigurationManager.AppSettings["StripeWebhookSecret"]
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
                _orderEntity = orderEntityList.FirstOrDefault(x => x.TrackingCode == paymentIndentId);
                _orderEntity.OrderState = OrderState.Canceled;
                _orderEntity.PaymentState = PaymentState.Refunded;
                await _orderService.UpdateAsync(_orderEntity);

                // Send an email to the Admin to infrom them of the successful refund
                _emailService.SendEmail("Payment Refund Successful!", 
                    $"Order with ID of #{_orderEntity.Id:00000} and payment ID of {_orderEntity.TrackingCode} was successfully refunded." +
                    $"<br />Transaction number: {refund.ReceiptNumber}", 
                    WebConfigurationManager.AppSettings["AdminEmail"]);

                // Send the invoice to the user as an email
                var emailBody = EmailTemplate.PrepareUpdate("Order Canceled And Refunded!", $"Your order with the ID of #{_orderEntity.Id:00000} has been canceled" +
                    $"and your payment has been fully refunded with the transaction number being {refund.ReceiptNumber}. " +
                    $"<br /> <b>Reason for refund: </b> {refund.Reason}", "");

                _emailService.SendEmail("Order Canceled And Refunded Successfully!", emailBody, _orderEntity.User.Email);
            }

            //always have to return 200
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        #endregion


        #region Conversion and Helper Methods 

        private void OrderEntityToModel(Order entity, ref OrderModel model, int index)
        {
            model.Id = entity.Id;
            model.Index = index;
            model.CreateDateTime = entity.CreateDateTime;
            model.TrackingCode = entity.TrackingCode;
            model.FirstName = entity.User?.UserInformation?.FirstName ?? "";
            model.LastName = entity.User?.UserInformation?.LastName ?? "";
            model.Country = entity.User?.UserInformation?.Country ?? "";
            model.State = entity.User?.UserInformation?.State ?? "";
            model.City = entity.User?.UserInformation?.City ?? "";
            model.Address = entity.User?.UserInformation?.Address ?? "";
            model.ZipCode = entity.User?.UserInformation?.ZipCode ?? "";
            model.PhoneNumber = entity.User?.UserInformation?.PhoneNumber ?? "";
            model.ShippingCost = entity.ShippingCost;
            model.DeliveryMethod = entity.DeliveryMethod;
            model.PaymentState = entity.PaymentState;
            model.OrderState = entity.OrderState;
            model.UserId = entity.UserId;
            model.Coupon = entity.Coupon;
            model.CouponId = entity.CouponId;

            foreach (var item in entity.OrderItems)
            {
                model.OrderItems.Add(new OrderItemModel
                {
                    Id = item.Id,
                    CartId = model.Id,
                    ProductId = item.ProductId,
                    ProductTitle = item.Product.Title,
                    Discount = item.Discount,
                    Tax = item.Tax,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Total = item.Total,
                });
            }
        }


        #endregion
    }
}