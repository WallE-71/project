using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Product;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت تخفیف")]
    public class DiscountController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string ProductNotFound = "محصول درخواستی یافت نشد.";
        public DiscountController(IUnitOfWork uw, IMapper mapper)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مدیریت تخفیف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderDiscount(string productId)
        {
            var viewModel = new DiscountViewModel();
            viewModel.ProductId = productId;
            var discount = _uw.BaseService<Discount>().FindByConditionAsync(d => d.ProductId == productId).Result.FirstOrDefault();
            if (discount != null)
            {
                var product = await _uw.BaseService<Product>().FindByIdAsync(productId);
                if (product.Price < 1000)
                    ModelState.AddModelError(string.Empty, "قیمت محصول برای اعمال تخفیف مناسب نیست!");
                if (discount.EndDate < DateTime.Now)
                    ModelState.AddModelError(string.Empty, "مهلت تخفیف به پایان رسیده است!");
                viewModel = _mapper.Map<DiscountViewModel>(discount);
                viewModel.PersianStartDate = discount.StartDate?.ToString("yyyy/MM/dd");
                viewModel.PersianEndDate = discount.EndDate?.ToString("yyyy/MM/dd");
            }
            return PartialView("_RenderDiscount", viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertOrUpdateDiscount(DiscountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ProductId.HasValue())
                {
                    var product = await _uw.BaseService<Product>().FindByIdAsync(viewModel.ProductId);
                    if (product.Price < 1000)
                    {
                        ModelState.AddModelError(string.Empty, "قیمت محصول برای اعمال تخفیف مناسب نیست!");
                        return PartialView("_RenderDiscount", viewModel);
                    }
                }

                viewModel.EndDate = viewModel.PersianEndDate.ConvertShamsiToMiladi();
                viewModel.StartDate = viewModel.PersianStartDate.ConvertShamsiToMiladi();
                if (viewModel.EndDate < DateTime.Now)
                {
                    ModelState.AddModelError(string.Empty, "تاریخ پایان تخفیف را با دقت انتخاب نمایید!");
                    return PartialView("_RenderDiscount", viewModel);
                }

                var discounts = await _uw.BaseService<Discount>().FindByConditionAsync(d => d.ProductId == viewModel.ProductId);
                if (discounts.Count() == 0)
                {
                    var discount = new Discount();
                    if (!viewModel.ProductId.HasValue())
                        viewModel.ProductId = viewModel.DiscountCode;
                    await _uw.BaseService<Discount>().CreateAsync(_mapper.Map(viewModel, discount));
                }
                else
                {
                    viewModel.UpdateTime = DateTime.Now;
                    _uw.BaseService<Discount>().Update(_mapper.Map(viewModel, discounts.FirstOrDefault()));
                }
                await _uw.BaseService<Discount>().Commit();
                TempData["notification"] = OperationSuccess;
            }
            return PartialView("_RenderDiscount", viewModel);
        }

        //[HttpGet]
        //public IActionResult RenderSpecialDiscount(string[] insertGroup)
        //{
        //    var viewModel = new DiscountViewModel();
        //    if (insertGroup.Count() == 0)
        //        ModelState.AddModelError(string.Empty, "هیچ محصولی برای درج تخفیف انتخاب نشده است.");

        //    viewModel.InsertGroup = insertGroup;
        //    return PartialView("_RenderSpecialDiscount", viewModel);
        //}

        [HttpGet]
        public async Task<IActionResult> InsertGroupSpecialDiscount(string[] insertGroup)
        {
            var viewModel = new DiscountViewModel();
            if (insertGroup.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ محصولی برای درج تخفیف انتخاب نشده است.");
            else
            {
                var day = DateTime.Now.AddDays(1).DayOfWeek;
                viewModel.DiscountDay = day == DayOfWeek.Saturday ? "شنبه" :
                                        day == DayOfWeek.Sunday ? "یکشنبه" :
                                        day == DayOfWeek.Monday ? "دوشنبه" :
                                        day == DayOfWeek.Tuesday ? "سهشنبه" :
                                        day == DayOfWeek.Wednesday ? "چهارشنبه" :
                                        day == DayOfWeek.Thursday ? "پنجشنبه" : "جمعه";

                viewModel.StartDate = DateTime.Now.Date.AddDays(1);
                viewModel.EndDate = viewModel.StartDate.EndOfDay();

                var ids = new string[insertGroup.Length];
                foreach (var item in insertGroup)
                    ids = item.Split(',');

                foreach (var productId in ids)
                {
                    viewModel.Percent = 50;
                    viewModel.ProductId = productId;
                    var product = await _uw.BaseService<Product>().FindByIdAsync(productId);
                    var discounts = await _uw.BaseService<Discount>().FindByConditionAsync(d => d.ProductId == productId);
                    if (discounts.Count() != 0 && product.Price > 1000)
                    {
                        viewModel.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                        _uw.BaseService<Discount>().Update(_mapper.Map(viewModel, discounts.FirstOrDefault()));
                    }
                    else if (discounts.Count() == 0 && product.Price > 1000)
                    {
                        viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                        await _uw.BaseService<Discount>().CreateAsync(_mapper.Map<Discount>(viewModel));
                    }
                    await _uw.BaseService<Discount>().Commit();
                    viewModel.InsertTime = null;
                    viewModel.UpdateTime = null;
                }
                return Json("Success");
            }
            return BadRequest(ProductNotFound);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDiscount(string productId)
        {
            if (!productId.HasValue())
                return Ok("تخفیفی پیدا نشد!");
            else
            {
                var discount = await _uw.BaseService<Discount>().FindByIdAsync(productId);
                if (discount == null)
                    return BadRequest("محصول تخفیف ندارد!");
                else
                {
                    if (discount.EndDate == null || discount.EndDate < DateTime.Now)
                        return Ok("تخفیف برای حذف معتبر نیست!");

                    _uw.BaseService<Discount>().Delete(discount);
                    await _uw.BaseService<Discount>().Commit();
                    return Ok("Success");
                }
            }
        }
    }
}
