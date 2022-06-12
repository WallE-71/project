using System.Threading.Tasks;

namespace ShoppingStore.Application.Contracts
{
    public interface IInsertNewsletter
    {
        Task<string> SendMessage(string email);
    }
}
