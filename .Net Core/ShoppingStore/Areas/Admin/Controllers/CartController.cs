using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Application.ViewModels.Cart;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سبد خرید")]
    public class CartController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string CartNotFound = "سبد خرید یافت نشد.";
        public CartController(IMapper mapper, IUnitOfWork uw)
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

        [HttpGet, DisplayName("سبد خرید"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> GetCarts(string search, string order, int offset, int limit, string sort)
        {
            List<CartViewModel> cartViewModels;
            int total = _uw.BaseService<Cart>().CountEntities();
            if (limit == 0)
                limit = total;

            cartViewModels = await _uw.AdminCarts.GetPaginateCartsAsync(offset, limit, "Id", search);
            if (search != "")
                total = cartViewModels.Count();
            return Json(new { total = total, rows = cartViewModels });
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int cartId)
        {
            if (cartId == 0)
                ModelState.AddModelError(string.Empty, CartNotFound);
            else
            {
                var cart = await _uw.BaseService<Cart>().FindByIdAsync(cartId);
                if (cart == null)
                    ModelState.AddModelError(string.Empty, CartNotFound);
                else
                    return PartialView("_DeleteConfirmation", cart);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Cart model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, CartNotFound);
            else
            {
                var cart = await _uw.BaseService<Cart>().FindByIdAsync(model.Id);
                if (cart == null)
                    ModelState.AddModelError(string.Empty, CartNotFound);
                else
                {
                    cart.RemoveTime = DateTime.Now;
                    _uw.BaseService<Cart>().Update(cart);
                    await _uw.BaseService<Cart>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", cart);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ سبدی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var cart = await _uw.BaseService<Cart>().FindByIdAsync(int.Parse(splite));
                    cart.RemoveTime = DateTime.Now;
                    _uw.BaseService<Cart>().Update(cart);
                }
                await _uw.BaseService<Cart>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(CartNotFound);
        }
    }
}
