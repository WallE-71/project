using System.Threading.Tasks;

namespace ShoppingStore.Infrastructure.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
