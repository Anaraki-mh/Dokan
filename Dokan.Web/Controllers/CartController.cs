using Dokan.Core.Database;
using Dokan.Domain.Enums;
using Dokan.Domain.UsersAndRoles;
using Dokan.Domain.Website;
using Dokan.Services;
using Dokan.Web.Models;
using Dokan.WebEssentials;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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

        private CartModel _cartModel;
        private CartItemModel _cartItemModel;
        private Order _orderEntity;
        private OrderItem _orderItemEntity;

        private Random _random;

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

            _cartModel = new CartModel();
            _cartItemModel = new CartItemModel();
            _orderEntity = new Order();
            _orderItemEntity = new OrderItem();
            _random = new Random();
        }

        #endregion


        #region Methods

        [HttpGet]
        public async Task<ActionResult> GetCart(bool partial = false)
        {
            HttpCookie cartIdCookie = Request.Cookies["cartId"];
            int cartId = 0;

            // Try to check if the cookie exists and get the cart id from the it
            cartId = GetCookieValue(cartIdCookie);

            // If there is no order with this id...
            _orderEntity = await _orderService.FindByIdAsync(cartId);
            if (_orderEntity is null)
            {
                // Create an Order entity and a cookie and set the id of the order entity as the value of the cookie
                _orderEntity = new Order()
                {
                    UserId = User.Identity.GetUserId() ?? "",
                    PaymentState = PaymentState.Pending,
                    OrderState = OrderState.Pending
                };
                await _orderService.CreateAsync(_orderEntity);

                cartIdCookie = WriteCookie("cartId", string.Concat(_orderEntity.Id.ToString(), _random.Next(10000).ToString("D6")), 30);

                Response.Cookies.Add(cartIdCookie);
            }

            _cartItemModel = new CartItemModel();
            OrderEntityToModel(_orderEntity, ref _cartModel);

            if (partial == true)
                return View("_Cart", _cartModel);

            return View(_cartModel);
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
            _orderEntity = await _orderService.FindByIdAsync(cartId);

            if (_orderEntity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _orderItemEntity = _orderEntity.OrderItems.Find(x => x.ProductId == productId);

            if (_orderEntity is null)
                _orderItemEntity = new OrderItem()
                {
                    ProductId = productId,
                    OrderId = _orderEntity.Id,
                    Price = productEntity.Price,
                    Tax = productEntity.ProductCategory?.TaxCategory?.Tax ?? 0,
                    Discount = (productEntity.DiscountCategory?.Discount ?? 0),
                };

            _orderItemEntity.Quantity = number;
            _orderItemEntity.Total = _orderItemEntity.Quantity *
                (_orderItemEntity.Price +
                (_orderItemEntity.Price * _orderItemEntity.Tax / 100) -
                (_orderItemEntity.Price * _orderItemEntity.Discount / 100));

            if (number == 0)
            {
                _orderEntity.OrderItems.Remove(_orderItemEntity);
                await _orderItemService.DeleteAsync(_orderItemEntity.Id);
            }
            else
                _orderEntity.OrderItems.Add(_orderItemEntity);


            await _orderService.UpdateAsync(_orderEntity);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<ActionResult> ApplyCoupon(string couponCode)
        {
            // Get the cartId from the cookie
            HttpCookie cartIdCookie = Request.Cookies["cartId"];
            int cartId = 0;
            cartId = GetCookieValue(cartIdCookie);

            // Try to find the Order entity by cartId and check if its null or already has a CouponId
            _orderEntity = await _orderService.FindByIdAsync(cartId);

            if (_orderEntity is null || _orderEntity?.CouponId != null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find the Coupon by its code and check if it exists or whether or not the user has already used this coupon/discount code
            var couponEntity = await _couponService.SearchAsync(couponCode);

            if (couponEntity is null ||
                couponEntity?.Users?.FirstOrDefault(x => x.Id == User.Identity.GetUserId()) != null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Add current user to the coupon users and increment its usage count
            var user = await _userService.FindByIdAsync(User.Identity.GetUserId());
            couponEntity.Users.Add(user);
            couponEntity.UsageCount++;
            await _couponService.UpdateAsync(couponEntity);

            // Update the prices of all orderItems which have a ProductCategory that's in the list of couponEntity's discounted categories
            // and change the value of _orderEntity's CouponId to the id of the used coupon
            foreach (var item in _orderEntity.OrderItems)
            {
                if (!couponEntity.ProductCategories.Contains(item.Product.ProductCategory))
                    continue;

                item.Discount = item.Discount + couponEntity.Discount > 100 ? 100 : item.Discount + couponEntity.Discount;
                item.Total = item.Quantity *
                    (item.Price +
                    (item.Price * item.Tax / 100) -
                    (item.Price * item.Discount / 100));
            }
            _orderEntity.CouponId = couponEntity.Id;
            await _orderService.UpdateAsync(_orderEntity);

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
            _orderEntity = await _orderService.FindByIdAsync(cartId);

            if (_orderEntity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Convert Order to CartItemModel
            _cartItemModel = new CartItemModel();
            OrderEntityToModel(_orderEntity, ref _cartModel);

            // To update the shipping cost in the list when the delivery method dropdown changes 
            ViewBag.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);

            return View(_cartModel);
        }

        [HttpPost]
        public async Task<ActionResult> Checkout(CartModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Get the cartId from the cookie
            HttpCookie cartIdCookie = Request.Cookies["cartId"];
            int cartId = 0;
            cartId = GetCookieValue(cartIdCookie);

            // Try to find the Order entity by cartId and check if its null 
            _orderEntity = await _orderService.FindByIdAsync(cartId);

            if (_orderEntity is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Create an account if the user doesnt have an account
            if ((_orderEntity.UserId == "" || _orderEntity.UserId is null) && _userManager.FindByEmail(model.Email) is null)
            {
                var user = new User
                {
                    UserName = model.FirstName + model.LastName,
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
                _userManager.Create(user, model.Password);
            }
            else
            {
                ViewBag.ErrorMessage = "This Email is already in use, please log on if you aleady have an account with this email or create a new account";
                return View(model);
            }


            // Save the billing and shipping info 
            _orderEntity.FirstName = model.FirstName;
            _orderEntity.LastName = model.LastName;
            _orderEntity.Country = model.Country;
            _orderEntity.State = model.State;
            _orderEntity.City = model.City;
            _orderEntity.Address = model.Address;
            _orderEntity.PhoneNumber = model.PhoneNumber;
            _orderEntity.ZipCode = model.ZipCode;
            _orderEntity.DeliveryMethod = model.DeliveryMethod;

            switch (model.DeliveryMethod)
            {
                case DeliveryMethod.StandardShipping:
                    _orderEntity.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);
                    break;
                case DeliveryMethod.Pickup:
                    _orderEntity.ShippingCost = 0;
                    break;
                default:
                    _orderEntity.ShippingCost = Convert.ToInt32(WebConfigurationManager.AppSettings["StandardShippingCost"]);
                    break;
            }

            await _orderService.UpdateAsync(_orderEntity);

            // Create the payment page using Stripe 
            List<SessionLineItemOptions> lineItems = new List<SessionLineItemOptions>();
            foreach (var item in _orderEntity.OrderItems)
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

            // The shipping cost
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = Convert.ToInt32(_orderEntity.ShippingCost) * 100,
                    Currency = "USD",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Shipping"
                    },
                    TaxBehavior = "unspecified"
                },
                AdjustableQuantity = new SessionLineItemAdjustableQuantityOptions
                {
                    Enabled = false,
                },
                Quantity = 1
            });

            var domain = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

            var options = new SessionCreateOptions
            {
                Metadata = new Dictionary<string, string>()
                {
                    ["OrderId"] = _orderEntity.Id.ToString(),
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{domain}/Cart/Success",
                CancelUrl = $"{domain}/Cart/Failure",
                CustomerEmail = _orderEntity.User.Email,
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
                    WebConfigurationManager.AppSettings["StripeWebhookSecret"]
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
                string orderId = "";

                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                session.Metadata.TryGetValue("OrderId", out orderId);

                // If there are no matching orders with the payment... 
                if(orderId == "")
                {
                    // send email to the admin to report the error
                    _emailService.SendEmail("Error! New payment with no matching order", 
                        $"A new payment with the session id of {session.Id} has been processed that has no matching order. " +
                        $"Please refund the payment and/or contact the user (user's email: {session.CustomerEmail}).",
                        WebConfigurationManager.AppSettings["AdminEmail"]);

                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }

                // Update the status of the order
                _orderEntity = await _orderService.FindByIdAsync(Convert.ToInt32(orderId));
                _orderEntity.OrderState = OrderState.Placed;
                _orderEntity.PaymentState = PaymentState.Paid;
                _orderEntity.TrackingCode = session.Id;
                await _orderService.UpdateAsync(_orderEntity);

                // Send the invoice to the user as an email
                string productsList = "";
                foreach (var item in _orderEntity.OrderItems)
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
                    $"{_orderEntity.Address}, {_orderEntity.City}, {_orderEntity.State}, {_orderEntity.Country}, {_orderEntity.ZipCode}",
                    "-",
                    _orderEntity.Id.ToString("D5"),
                    productsList,
                    _orderEntity.OrderItems.Sum(x => x.Total).ToString("N0"),
                    _orderEntity.OrderItems.Sum(x => x.Price * x.Tax).ToString("N0"),
                    _orderEntity.ShippingCost.ToString("N0"),
                    (_orderEntity.OrderItems.Sum(x => x.Total) + _orderEntity.ShippingCost).ToString("N0"));

                _emailService.SendEmail("Payment Successful!", emailBody, _orderEntity.User.Email);

                Console.WriteLine($"checkout session with ID {session.Id} completed.");
            }

            //always have to return 200
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #endregion


        #region Conversion and Helper Methods

        private void OrderEntityToModel(Order entity, ref CartModel model)
        {
            model.Id = entity.Id;
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
            model.Coupon = entity.Coupon ?? new Domain.Website.Coupon();
            model.CouponId = entity.CouponId;

            foreach (var item in entity.OrderItems)
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
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Total = item.Total,
                });
            }
        }

        private void OrderItemEntityToModel(BlogPost entity, ref BlogPostModel model)
        {
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.ShortDescription = entity.ShortDescription;
            model.Content = entity.Content;
            model.Image = entity.Image;
            model.CategoryId = entity.BlogCategoryId;
            model.CategoryTitle = entity.BlogCategory?.Title ?? " - ";
            model.CreateDateTime = $"{entity.CreateDateTime:MMM d - yyyy}";
        }


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