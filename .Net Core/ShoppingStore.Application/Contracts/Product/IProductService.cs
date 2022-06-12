using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingStore.Application.Contracts
{
    public interface IProductService
    {
        Task<string> GetMaxDiscountTypeInCategory();

        Task<List<string>> GetProductImagesAsync(string productId, int offset, int limit);
        Task<int?> DiscountManagerAsync(string productId, bool save = false);
        Task ProductStateManagerAsync(string productId);
        Task<string> GetProductImage(string productId);
    }
}
