using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Order;
using ShoppingStore.Domain.Common.Api.Attributes;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("OrderApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class OrderApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public OrderApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpGet]
        public virtual ApiResult<List<UserOrderViewModel>> GetOrders(int userId, int offset, int limit)
        {
            if (userId != 0)
                return Ok(_uw.SiteOrders.GetUserOrdersAsync(userId, offset, limit).Result.Data);
            return null;
        }

        [HttpPost]
        public virtual ResultViewModel RemoveOrder(int orderId, int userId)
        {
            if (userId != 0 && orderId != 0)
                return _uw.SiteOrders.RemoveOrder(orderId, userId);
            return null;
        }
    }
}
