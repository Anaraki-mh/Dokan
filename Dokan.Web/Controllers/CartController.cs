using Dokan.Core.Database;
using Dokan.Domain.Enums;
using Dokan.Domain.UsersAndRoles;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Helpers;
using Dokan.Web.Models;
using Dokan.WebEssentials;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;

namespace Dokan.Web.Controllers
{
    public class CartController : Controller
    {
        #region Fields and Properties

        private IProductService _productService { get; }
        private IOrderItemService _orderItemService { get; }
        private IOrderService _orderService { get; }
        private ICouponService _couponService { get; }
        private IEmailService _emailService { get; }
        private IUserService _userService { get; }

        private UserStore<User> _userStore { get; }
        private UserManager<User> _userManager { get; }

        #endregion


        #region Constructor

        public CartController(IProductService productService,
            IOrderItemService orderItemService,
            IOrderService orderService,
            ICouponService couponService,
            IEmailService emailService,
            IUserService userService,
            DokanContext context)
        {
            _productService = productService;
            _orderItemService = orderItemService;
            _orderService = orderService;
            _couponService = couponService;
            _emailService = emailService;
            _userService = userService;

            _userStore = new UserStore<User>(context);
            _userManager = new UserManager<User>(_userStore);

            LayoutHelper.PrepareLayout();
        }

        #endregion


        #region Methods

        [HttpGet]
        public async Task<ActionResult> ShoppingCart(bool partial = false)
        {
            HttpCookie cartIdCookie = Request.Cookies["cartId"];
            int cartId = 0;

            // Try to check if the cookie exists and get the cart id from the it
            cartId = GetCookieValue(cartIdCookie);

            // If there is no order with this id...
            var orderEntity = await _orderService.FindByIdAsync(cartId);
            if (orderEntity is null || orderEntity?.OrderState != OrderState.Pending)
            {
                // Create an Order entity and a cookie and set the id of the order entity as the value of the cookie
                orderEntity = new Order()
                {
                    UserId = User.Identity.GetUserId() ?? _userManager.FindByEmail("Guest@Dokan.com").Id,
                    PaymentState = PaymentState.Pending,
                    OrderState = OrderState.Pending,
                    DeliveryMethod = DeliveryMethod.StandardShipping,
                    ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"])
                };
                await _orderService.CreateAsync(orderEntity);

                var random = new Random();
                cartIdCookie = WriteCookie("cartId", string.Concat(orderEntity.Id.ToString(), random.Next(10000).ToString("D4")), 30);

                Response.Cookies.Add(cartIdCookie);
            }

            var cartModel = CartModel.EntityToModel(in orderEntity);

            if (partial == true)
                return PartialView("_ShoppingCart", cartModel);

            return View(cartModel);
        }

        [HttpGet]
        public async Task<ActionResult> Add(int productId, int number)
        {
            // Check if any of the parameters have an invalid value
            if (productId == 0 || number < 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find the product and check if a product with this id exists
            var productEntity = await _productService.FindByIdAsync(productId);

            if (productEntity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Get the cartId from the cookie 
            HttpCookie cartIdCookie = Request.Cookies["cartId"];
            int cartId = 0;
            cartId = GetCookieValue(cartIdCookie);

            // Try to find the Order entity with cartId and check if exists
            var orderEntity = await _orderService.FindByIdAsync(cartId);

            if (orderEntity is null || orderEntity?.OrderState != OrderState.Pending)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var orderItemEntity = orderEntity.OrderItems.Find(x => x.ProductId == productId);

            if (orderItemEntity is null)
                orderItemEntity = new OrderItem()
                {
                    ProductId = productId,
                    OrderId = orderEntity.Id,
                    Price = productEntity.Price,
                    Tax = productEntity.ProductCategory?.TaxCategory?.Tax ?? 0,
                    Discount = (productEntity.DiscountCategory?.Discount ?? 0),
                };

            orderItemEntity.Quantity = number;
            orderItemEntity.Total = orderItemEntity.Quantity *
                (orderItemEntity.Price +
                (orderItemEntity.Price * orderItemEntity.Tax / 100) -
                (orderItemEntity.Price * orderItemEntity.Discount / 100));

            if (number == 0)
            {
                orderEntity.OrderItems.Remove(orderItemEntity);
                await _orderItemService.DeleteAsync(orderItemEntity.Id);
            }
            else
                orderEntity.OrderItems.Add(orderItemEntity);


            await _orderService.UpdateAsync(orderEntity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> ApplyCoupon(string couponCode)
        {
            if (couponCode == "")
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Get the cartId from the cookie
            HttpCookie cartIdCookie = Request.Cookies["cartId"];
            int cartId = 0;
            cartId = GetCookieValue(cartIdCookie);

            // Try to find the Order entity by cartId and check if its null or already has a CouponId
            var orderEntity = await _orderService.FindByIdAsync(cartId);

            if (orderEntity is null || orderEntity?.CouponId != null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find the Coupon by its code and check if it exists or whether or not the user has already used this coupon/discount code
            var couponEntity = await _couponService.SearchAsync(couponCode);


            var userid = User.Identity.GetUserId();


            if (couponEntity?.Id == 0 ||
                couponEntity?.Users?.FirstOrDefault(x => x.Id == User.Identity.GetUserId()) != null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Add current user to the coupon users and increment its usage count
            var user = await _userService.FindByIdAsync(User.Identity.GetUserId());

            if (user?.Id != _userManager.FindByEmail("Guest@Dokan.com").Id)
                couponEntity.Users.Add(user);
            
            couponEntity.UsageCount++;
            await _couponService.UpdateAsync(couponEntity);

            // Update the prices of all orderItems which have a ProductCategory that's in the list of couponEntity's discounted categories
            // and change the value of _orderEntity's CouponId to the id of the used coupon
            foreach (var item in orderEntity.OrderItems)
            {
                if (!couponEntity.ProductCategories.Contains(item.Product.ProductCategory))
                    continue;

                item.Discount = item.Discount + couponEntity.Discount > 100 ? 100 : item.Discount + couponEntity.Discount;
                item.Total = item.Quantity *
                    (item.Price +
                    (item.Price * item.Tax / 100) -
                    (item.Price * item.Discount / 100));
            }
            orderEntity.CouponId = couponEntity.Id;
            await _orderService.UpdateAsync(orderEntity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> Checkout()
        {
            // Get the cartId from the cookie
            HttpCookie cartIdCookie = Request.Cookies["cartId"];
            int cartId = 0;
            cartId = GetCookieValue(cartIdCookie);

            // Try to find the Order entity by cartId and check if its null 
            var orderEntity = await _orderService.FindByIdAsync(cartId);

            if (orderEntity is null || orderEntity?.OrderState != OrderState.Pending)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Convert Order to CartModel
            var cartModel = CartModel.EntityToModel(in orderEntity);
            cartModel.Country = "United States";

            // To update the shipping cost in the list when the delivery method dropdown changes 
            ViewBag.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);

            return View(cartModel);
        }

        [HttpPost]
        public async Task<ActionResult> Checkout(CartModel model)
        {
            // Get the cartId from the cookie
            HttpCookie cartIdCookie = Request.Cookies["cartId"];
            int cartId = 0;
            cartId = GetCookieValue(cartIdCookie);

            // Try to find the Order entity by cartId and check if its null 
            var orderEntity = await _orderService.FindByIdAsync(cartId);

            if (orderEntity is null || orderEntity?.OrderState != OrderState.Pending)
            {
                ViewBag.ErrorMessage = "Invalid Order. This order does not exist or has already been processed.";

                // To update the shipping cost in the list when the delivery method dropdown changes 
                ViewBag.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);
                foreach (var item in orderEntity.OrderItems)
                {
                    model.CartItems.Add(new CartItemModel
                    {
                        Id = item.Id,
                        CartId = model.Id,
                        ProductId = item.ProductId,
                        ProductImage = item.Product.Image1,
                        ProductTitle = item.Product.Title,
                        Discount = item.Discount,
                        Tax = item.Tax,
                        Price = $"{item.Total / (double)item.Quantity:0.00}",
                        Quantity = item.Quantity,
                        Total = item.Total,
                    });
                }
                return View(model);
            }

            string userId = User.Identity.GetUserId();
            string guestUserId = _userManager.FindByEmail("Guest@Dokan.com").Id;
            bool isGuest = userId == guestUserId || userId is null ? true : false;

            if (isGuest == false && !(orderEntity.UserId == userId || orderEntity.UserId == guestUserId))
            {
                ViewBag.ErrorMessage = "Invalid Checkout Request. Orders can not be checked out by any user other than the one associated with the order.";

                // To update the shipping cost in the list when the delivery method dropdown changes 
                ViewBag.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);
                return View(model);
            }

            // Create an account if the user doesnt have an account
            if (isGuest && (model.Password != string.Empty || model.Password != null))
            {
                var user = new User
                {
                    UserName = model.FirstName + " " + model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserInformation = new UserInformation
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Country = model.Country,
                        State = model.State,
                        City = model.City,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        ZipCode = model.ZipCode
                    }
                };
                user.UsedCoupons.Add(orderEntity.Coupon);

                _userManager.Create(user, model.Password);
                orderEntity.User = user;
            }
            else
            {
                ViewBag.ErrorMessage = "This Email is already in use, please log on if you aleady have an account with this email or create a new account";
                return View(model);
            }
            
            // Save the billing and shipping info 
            orderEntity.FirstName = model.FirstName;
            orderEntity.LastName = model.LastName;
            orderEntity.Country = model.Country;
            orderEntity.State = model.State;
            orderEntity.City = model.City;
            orderEntity.Address = model.Address;
            orderEntity.PhoneNumber = model.PhoneNumber;
            orderEntity.ZipCode = model.ZipCode;
            orderEntity.DeliveryMethod = model.DeliveryMethod;

            switch (model.DeliveryMethod)
            {
                case DeliveryMethod.StandardShipping:
                    orderEntity.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);
                    break;
                case DeliveryMethod.Pickup:
                    orderEntity.ShippingCost = 0;
                    break;
                default:
                    orderEntity.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);
                    break;
            }

            await _orderService.UpdateAsync(orderEntity);

            // Create the payment page using Stripe 
            List<SessionLineItemOptions> lineItems = new List<SessionLineItemOptions>();
            foreach (var item in orderEntity.OrderItems)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = Convert.ToInt32(item.Price + (item.Price * item.Tax / 100) - (item.Price * item.Discount / 100)) * 100,
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                            Description = $"%{item.Tax} tax included"
                        },
                        TaxBehavior = "inclusive"
                    },
                    AdjustableQuantity = new SessionLineItemAdjustableQuantityOptions
                    {
                        Enabled = false,
                    },
                    Quantity = item.Quantity
                });
            }

            var domain = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

            var options = new SessionCreateOptions
            {
                ClientReferenceId = orderEntity.Id.ToString(),
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{domain}/Cart/Success",
                CancelUrl = $"{domain}/Cart/Failure",
                CustomerEmail = orderEntity.User.Email,
                ShippingOptions = new List<SessionShippingOptionOptions>
                {
                    new SessionShippingOptionOptions
                    {
                        ShippingRateData = new SessionShippingOptionShippingRateDataOptions
                        {
                            Type = "fixed_amount",
                            FixedAmount = new SessionShippingOptionShippingRateDataFixedAmountOptions
                            {
                                Amount = Convert.ToInt32(orderEntity.ShippingCost) * 100,
                                Currency = "usd",
                            },
                            DisplayName = "Shipping",
                            DeliveryEstimate = new SessionShippingOptionShippingRateDataDeliveryEstimateOptions
                            {
                                Minimum = new SessionShippingOptionShippingRateDataDeliveryEstimateMinimumOptions
                                {
                                    Unit = "business_day",
                                    Value = 3,
                                },
                                Maximum = new SessionShippingOptionShippingRateDataDeliveryEstimateMaximumOptions
                                {
                                    Unit = "business_day",
                                    Value = 7,
                                },
                            },
                        },
                    },
                }
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new HttpStatusCodeResult(303);
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
                    WebConfigurationManager.AppSettings["StripeSessionCompleteWebhookSecret"]
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Faild to unmarshal event data. /n {e}");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //automate based on event type
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                string orderId = session.ClientReferenceId;

                // If there are no matching orders with the payment... 
                if (orderId is null)
                {
                    // send email to the admin to report the error
                    _emailService.SendEmail("Error! New payment with no matching order",
                        $"A new payment with the session id of {session.Id} and payment id of {session.PaymentIntentId} has been processed that has no matching order. " +
                        $"Please refund the payment and/or contact the user (user's email: {session.CustomerEmail}).",
                        WebConfigurationManager.AppSettings["AdminEmail"]);

                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }

                // Update the status of the order
                var orderEntity = await _orderService.FindByIdAsync(Convert.ToInt32(orderId));
                orderEntity.OrderState = OrderState.Placed;
                orderEntity.PaymentState = PaymentState.Paid;
                orderEntity.TrackingCode = session.PaymentIntentId;
                await _orderService.UpdateAsync(orderEntity);

                // Send the invoice to the user as an email
                string productsList = "";
                foreach (var item in orderEntity.OrderItems)
                {
                    productsList += EmailTemplate.CreateInvoiceProductRow
                            ($"{EmailTemplate.WebAddress}/Products/{item.Id}/{SEO.CreateSeoFriendlyUrlTitle(item.Product.Title)}",
                            item.Product.Image1,
                            item.Product.Title,
                            item.Quantity,
                            item.Price);
                }

                string emailBody = EmailTemplate.PrepareInvoice("Payment Successful!",
                    "Your order has been placed and the payment has been successful; you will recieve updates on the status of the order through email.",
                    "Order Details",
                    EmailTemplate.WebAddress,
                    $"{orderEntity.Address}, {orderEntity.City}, {orderEntity.State}, {orderEntity.Country}, {orderEntity.ZipCode}",
                    "-",
                    orderEntity.Id.ToString("D5"),
                    productsList,
                    orderEntity.OrderItems.Sum(x => x.Total).ToString("N0"),
                    orderEntity.OrderItems.Sum(x => x.Price * x.Tax).ToString("N0"),
                    orderEntity.ShippingCost.ToString("N0"),
                    (orderEntity.OrderItems.Sum(x => x.Total) + orderEntity.ShippingCost).ToString("N0"));

                _emailService.SendEmail("Payment Successful!", emailBody, orderEntity.User.Email);

                Console.WriteLine($"checkout session with ID {session.Id} completed.");
            }

            //always have to return 200
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Failure()
        {
            return View();
        }

        #endregion


        #region Helper Methods

        private int GetCookieValue(HttpCookie cookie)
        {
            int value = 0;

            if (cookie is null)
                return value;

            string cartIdCookieValue = cookie.Value.Split('=')[1];
            Int32.TryParse(cartIdCookieValue.Substring(0, cartIdCookieValue.Length - 4), out value);

            return value;
        }

        private HttpCookie WriteCookie(string name, string value, int expiresAfterDays)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Values[name] = value;
            cookie.Expires = DateTime.Now.AddDays(expiresAfterDays);

            return cookie;
        }

        #endregion
    }
}