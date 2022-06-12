using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Api.Attributes;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("RateApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class RateApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public RateApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpPost]
        public virtual async Task<ApiResult<object>> RateToSeller(int sellerId, string productId, string userId, int rate)
        {
            int id;
            Int32.TryParse(userId, out id);
            if (productId.HasValue() && userId != null)
                return Ok(await _uw.InsertRateSaller.RateToSeller(sellerId, productId, id, rate));
            return Ok();
        }
    }
}
