using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Seller;

namespace ShoppingStore.Application.Contracts
{
    public interface ISiteSellers
    {
        Task<SellerViewModel> GetSellerForProductAsync(string productId);
    }
}
