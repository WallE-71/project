using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.CreditCart;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت کارت های اعتباری")]
    public class CreditCartController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string CreditCartNotFound = "کارت اعتباری یافت نشد.";
        public CreditCartController(IMapper mapper, IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet, DisplayName("کارت اعتباری"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> GetCreditCarts(string search, string order, int offset, int limit, string sort)
        {
            List<CreditCartViewModel> viewModels;
            int total = _uw.BaseService<CreditCart>().CountEntities();
            if (limit == 0)
                limit = total;

            if (sort == "credit")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminCreditCarts.GetPaginateCreditCartsAsync(offset, limit, "Credit", search);
                else
                    viewModels = await _uw.AdminCreditCarts.GetPaginateCreditCartsAsync(offset, limit, "Credit desc", search);
            }
            else if (sort == "name")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminCreditCarts.GetPaginateCreditCartsAsync(offset, limit, "Name", search);
                else
                    viewModels = await _uw.AdminCreditCarts.GetPaginateCreditCartsAsync(offset, limit, "Name desc", search);
            }
            else
                viewModels = await _uw.AdminCreditCarts.GetPaginateCreditCartsAsync(offset, limit, "Credit desc", search);

            if (search != "")
                total = viewModels.Count();
            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string creditCartId)
        {
            if (!creditCartId.HasValue())
                ModelState.AddModelError(string.Empty, CreditCartNotFound);
            else
            {
                var creditCart = await _uw.BaseService<CreditCart>().FindByIdAsync(creditCartId);
                if (creditCart == null)
                    ModelState.AddModelError(string.Empty, CreditCartNotFound);
                else
                    return PartialView("_DeleteConfirmation", creditCart);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(string owner)
        {
            if (owner.HasValue())
                ModelState.AddModelError(string.Empty, CreditCartNotFound);
            else
            {
                var creditCart = _uw.BaseService<CreditCart>().FindByConditionAsync(c => c.Owner == owner).Result.FirstOrDefault();
                if (creditCart == null)
                    ModelState.AddModelError(string.Empty, CreditCartNotFound);
                else
                {
                    creditCart.RemoveTime = DateTime.Now;
                    _uw.BaseService<CreditCart>().Update(creditCart);
                    await _uw.BaseService<CreditCart>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", creditCart);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ کارتی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var creditCart = await _uw.BaseService<CreditCart>().FindByIdAsync(int.Parse(splite));
                    creditCart.RemoveTime = DateTime.Now;
                    _uw.BaseService<CreditCart>().Update(creditCart);
                }
                await _uw.BaseService<CreditCart>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(CreditCartNotFound);
        }
    }
}
