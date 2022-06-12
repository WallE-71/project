using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Dashboard;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("داشبورد")]
    public class DashboardController : BaseController
    {
        private readonly IUnitOfWork _uw;
        public DashboardController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Index()
        {
            ViewBag.IsOffline = CheckOffline();

            ViewBag.TodaySaleAmount = _uw.AdminOrders.TodaySaleAmount();
            ViewBag.CountProductSale = _uw.AdminProducts.CountProductSale();
            ViewBag.ProductsPresentation = _uw.AdminProducts.CountProductPresentation();

            DateTime EndDateTimeMiladi;
            DateTime StartDateTimeMiladi;
            var month = StringExtensions.GetMonth();
            var numberOfVisitList = new List<NumberOfVisitChartViewModel>();
            var year = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "yyyy");

            for (int i = 0; i < month.Length; i++)
            {
                StartDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/{i + 1}/01");
                if (i < 11)
                    EndDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/{i + 2}/01");
                else
                    EndDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/01/01");

                var numberOfVisit = await _uw.AdminProducts.GetNumberOfVisitAsync(EndDateTimeMiladi, StartDateTimeMiladi);
                numberOfVisitList.Add(new NumberOfVisitChartViewModel { Name = month[i], Value = numberOfVisit });
            }
            ViewBag.NumberOfVisitChart = numberOfVisitList;
            return View();
        }
    }
}
