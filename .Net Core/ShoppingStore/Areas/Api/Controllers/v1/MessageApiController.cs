using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Domain.Common.Api.Attributes;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("MessageApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class MessageApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IApplicationUserManager _userManager;
        public MessageApiController(
            IUnitOfWork uw,
            IApplicationUserManager userManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }

        [HttpPost]
        public async Task<ApiResult<string>> SendMessage(string email, string description, int typeFeedBack)
        {
            if (email.HasValue() && description.HasValue() && typeFeedBack != 0)
                return Ok(await _uw.InsertMassageOfUser.SendMessage(email, description, typeFeedBack));
            return Ok("فیلدهای ورودی معتبر نیست!");
        }

        [HttpGet]
        public async Task<ApiResult<object>> ReciveAnswer(string email)
        {
            var customer = await _userManager.FindByEmailAsync(email);
            if (customer == null)
                return Ok();

            var messageUser = await _uw.BaseService<MessageUser>().FindByConditionAsync(m => m.UserId == customer.Id);
            if (messageUser.FirstOrDefault().Answer == null)
                return Ok("پاسخی داده نشده است");
            else
            {
                var author = await _uw.BaseService<User>().FindByConditionAsync(u => u.Id == messageUser.FirstOrDefault().User.Id);
                return Ok(new { answer = messageUser.FirstOrDefault().Answer, author = author.FirstOrDefault() } );
            }
        }
    }
}
