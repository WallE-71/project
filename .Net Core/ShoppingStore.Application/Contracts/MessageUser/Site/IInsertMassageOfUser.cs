using System.Threading.Tasks;

namespace ShoppingStore.Application.Contracts
{
    public interface IInsertMassageOfUser
    {
        Task<string> SendMessage(string email, string description, int typeFeedBack);
    }
}
