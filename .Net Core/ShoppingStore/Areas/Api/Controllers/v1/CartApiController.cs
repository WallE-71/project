using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Cart;
using ShoppingStore.Domain.Common.Api.Attributes;
using System.Collections.Generic;
using ShoppingStore.Domain.Entities.Identity;
using System.Linq;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("CartApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class CartApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public CartApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpGet]
        public async Task<ApiResult<CartViewModel>> Cart(string phoneNumber, string browserId)
        {
            var users = await _uw.BaseService<User>().FindByConditionAsync(u => u.PhoneNumber == phoneNumber);
            if (users.Count() == 0) return new CartViewModel();
            var result = await _uw.SiteCarts.GetCartAsync(users.FirstOrDefault().Id, browserId);

            var viewModel = await _uw.ManageShoppingCart.ManageCart(result, browserId);
            foreach (var item in viewModel.CartItems)
            {
                item.Colors = new List<string>();
                foreach (var color in item.NameOfColor.Trim().Replace(" ", "").Split(","))
                    if (color != "")
                        item.Colors.Add(color);
                item.Image = await _uw.AdminFiles.FindImageAsync(item.ProductId, null, null, null);
            }
            return viewModel;
        }

        [HttpPost]
        public virtual async Task<ApiResult<object>> Post(string productId, string browserId, string color)
        {
            if (browserId != "null")
                return Ok(await _uw.SiteCarts.AddToCartAsync(productId, browserId, color));
            else
                return Ok();
        }

        [HttpPut]
        public virtual async Task<ApiResult<IActionResult>> Increase(int cartItemId)
        {
            return Ok(await _uw.SiteCarts.Increase(cartItemId));
        }

        [HttpPut("Decrease")]
        public virtual async Task<ApiResult<IActionResult>> Decrease(int cartItemId)
        {
            return Ok(await _uw.SiteCarts.Decrease(cartItemId));
        }

        [HttpDelete("Delete")]
        public virtual async Task<ApiResult<object>> Delete(string productId, string browserId)
        {
            if (browserId != "null")
                return Ok(await _uw.SiteCarts.RemoveFromCart(productId, browserId));
            else
                return Ok();
        }

        [HttpDelete("DeleteAll")]
        public virtual async Task<ApiResult<object>> DeleteAll(string browserId)
        {
            if (browserId != "null")
                return Ok(await _uw.SiteCarts.RemoveAllFromCart(browserId));
            else
                return Ok();
        }
    }
}
