using System.Threading.Tasks;

namespace ShoppingStore.Application.Contracts
{
    public interface IManageCreditCartUser
    {
        Task<object> CreditCartAsync(string phoneNumber, int credit, string nationalId, string bankCode, bool getCart);
    }
}
