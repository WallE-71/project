using System.Threading.Tasks;

namespace ShoppingStore.Application.Contracts
{
    public interface IInsertRateSaller
    {
        Task<bool> RateToSeller(int sellerId, string productId, int userId, int rate);
    }
}
