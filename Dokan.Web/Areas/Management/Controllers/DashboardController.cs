using Dokan.Domain.Enums;
using Dokan.Domain.Website;
using Dokan.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dokan.Web.Areas.Management.Controllers
{
    public class DashboardController : ManagementBaseController
    {

        #region Fields and Properties

        private IOrderService _orderService { get; }
        private IOrderItemService _orderItemService { get; }
        private IDiscountCategoryService _discountCategoryService { get; }
        private ICouponService _couponService { get; }


        #endregion


        #region Constructor

        public DashboardController(IOrderService orderService, IDiscountCategoryService discountCategoryService, ICouponService couponService, IOrderItemService orderItemService)
        {
            _orderService = orderService;
            _discountCategoryService = discountCategoryService;
            _couponService = couponService;
            _orderItemService = orderItemService;
        }

        #endregion


        #region Methods

        public async Task<ActionResult> Overview()
        {
            var now = DateTime.UtcNow;

            var paidForOrders = await _orderService.ListAsync();
            paidForOrders = paidForOrders.Where(x => x.CreateDateTime.Year == now.Year && x.PaymentState == PaymentState.Paid).ToList();

            string chartLabels = "";
            string chartData = "";

            for (int i = 1; i <= 12; i++)
            {
                chartData += $"{paidForOrders.Where(x => x.CreateDateTime.Month == i).Sum(x => x?.OrderItems?.Sum(y => y?.Total)) ?? 0.00} ,";
                chartLabels += $"\"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i)}\" ,";
            }

            ViewBag.ChartLabels = chartLabels;
            ViewBag.ChartData = chartData;

            ViewBag.TotalSalesOfYear = paidForOrders.Sum(x => x?.OrderItems?.Sum(y => y?.Total)).ToString() ?? "0.00";
            ViewBag.TotalSalesTaxOfYear = paidForOrders.Sum(x => x?.OrderItems?.Sum(y => y?.Price * y?.Tax / 100)).ToString() ?? "0.00";

            paidForOrders = paidForOrders.Where(x => x.CreateDateTime.Month == now.Month).ToList();
            ViewBag.TotalSalesOfMonth = paidForOrders.Sum(x => x?.OrderItems?.Sum(y => y?.Total)).ToString() ?? "0.00";

            paidForOrders = paidForOrders.Where(x => x.CreateDateTime.Day >= now.Day - 7 && (int)x.CreateDateTime.DayOfWeek <= (int)now.DayOfWeek).ToList();
            ViewBag.TotalSalesOfWeek = paidForOrders.Sum(x => x?.OrderItems?.Sum(y => y?.Total)).ToString() ?? "0.00";


            return View();
        }

        public async Task<ActionResult> BestSellingProducts()
        {
            var topTenOrderItems = await _orderItemService.ListAsync();
            topTenOrderItems = topTenOrderItems.Where(x => x.Order.PaymentState == PaymentState.Paid).OrderByDescending(x => x.Quantity).Take(10).ToList();

            var bestSellingProducts = topTenOrderItems.Select(x => x.Product);

            return View("_BestSellingProducts", bestSellingProducts);
        }

        public async Task<ActionResult> NewOrders()
        {
            var newOrders = await _orderService.ListAsync();
            newOrders = newOrders.Where(x => x.OrderState == OrderState.Placed).ToList();

            return View("_NewOrders", newOrders);
        }

        public async Task<ActionResult> OngoingDiscounts()
        {
            var ongoingDiscounts = await _discountCategoryService.ListAsync();
            ongoingDiscounts = ongoingDiscounts.Where(x => x.IsActive && x.ExpiryDateTime >= DateTime.UtcNow).ToList();

            return View("_OngoingDiscounts", ongoingDiscounts);
        }

        public async Task<ActionResult> ActiveCoupons()
        {
            var activeCoupons = await _couponService.ListAsync();
            activeCoupons = activeCoupons.Where(x => x.IsActive && x.ExpiryDateTime >= DateTime.UtcNow).ToList(); 

            return View("_ActiveCoupons", activeCoupons);
        }

        #endregion

    }
}