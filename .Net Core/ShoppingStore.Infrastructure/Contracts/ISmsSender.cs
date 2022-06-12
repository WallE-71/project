using System.Threading.Tasks;

namespace ShoppingStore.Infrastructure.Contracts
{
    public interface ISmsSender
    {
        Task<string> SendAuthSmsAsync(string Code, string PhoneNumber);
    }
}
