using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Infrastructure.Contracts;

namespace ShoppingStore.Infrastructure
{
    public class SendWeeklyNewsLetter : IInvocable
    {
        private readonly IEmailSender _emailSender;
        private readonly IProductFacade _productFacade;
        private readonly IBaseService<Newsletter> _baseNewsletterService;
        public SendWeeklyNewsLetter(
            IEmailSender emailSender,
            IProductFacade productFacade,
            IBaseService<Newsletter> baseNewsletterService)
        {
            _emailSender = emailSender;
            _productFacade = productFacade;
            _baseNewsletterService = baseNewsletterService;
        }

        public async Task Invoke()
        {
            var users = _baseNewsletterService.FindByConditionAsync(l => l.IsComplete == true).Result.ToList();
            var emailContent = await _productFacade.SiteProducts.GetWeeklyProductDiscountAsync(string.Format("/{0}:://{1}"));

            if (emailContent != "")
                foreach (var item in users)
                    await _emailSender.SendEmailAsync(item.Email, "اطلاعیه جدیدترین محصولات", emailContent);
        }
    }
}
