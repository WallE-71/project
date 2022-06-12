using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Api.Attributes;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("NewsletterApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class NewsLetterApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public NewsLetterApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpPost]
        public async Task<ApiResult<string>> SendMessage(string email)
        {
            if (email.HasValue())
                return Ok(await _uw.InsertNewsletter.SendMessage(email));
            return Ok();
        }
    }
}
