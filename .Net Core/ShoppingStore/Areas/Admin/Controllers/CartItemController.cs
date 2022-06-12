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
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Cart;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت موارد سبد خرید")]
    public class CartItemController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string CartItemNotFound = "آیتمی در سبد خرید یافت نشد.";
        public CartItemController(IMapper mapper, IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("محتوای سبد خرید"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult CartItems(int cartId)
        {
            return PartialView("_CartItemTable", new CartViewModel { Id = cartId });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems(string search, string order, int offset, int limit, string sort, int cartId)
        {
            List<CartItemViewModel> cartItemViewModels;
            int total = _uw.BaseService<CartItem>().CountEntities();
            if (limit == 0)
                limit = total;
            cartItemViewModels = await _uw.AdminCarts.GetPaginateCartItemsAsync(offset, limit, "Price", search, cartId);
            if (search != "")
                total = cartItemViewModels.Count();
            return Json(new { total = total, rows = cartItemViewModels });
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int cartItemId)
        {
            if (cartItemId == 0)
                ModelState.AddModelError(string.Empty, CartItemNotFound);
            else
            {
                var cartItem = await _uw.BaseService<CartItem>().FindByIdAsync(cartItemId);
                if (cartItem == null)
                    ModelState.AddModelError(string.Empty, CartItemNotFound);
                else
                    return PartialView("_DeleteConfirmation", cartItem);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(CartItem model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, CartItemNotFound);
            else
            {
                var cartItem = await _uw.BaseService<CartItem>().FindByIdAsync(model.Id);
                if (cartItem == null)
                    ModelState.AddModelError(string.Empty, CartItemNotFound);
                else
                {
                    cartItem.RemoveTime = DateTime.Now;
                    _uw.BaseService<CartItem>().Update(cartItem);
                    await _uw.BaseService<CartItem>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", cartItem);
                }
            }
            return RedirectToAction("CartItem");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ موردی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var cartItem = await _uw.BaseService<CartItem>().FindByIdAsync(int.Parse(splite));
                    cartItem.RemoveTime = DateTime.Now;
                    _uw.BaseService<CartItem>().Update(cartItem);
                }
                await _uw.BaseService<CartItem>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(CartItemNotFound);
        }
    }
}
