using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Api.Attributes;
using ShoppingStore.Application.ViewModels.Slider;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("SliderApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class SliderApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public SliderApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpGet]
        public virtual async Task<ApiResult<List<SliderViewModel>>> GetSliders()
        {
            return Ok(await _uw.SiteSliders.GetSlidersAsync());
        }
    }
}
