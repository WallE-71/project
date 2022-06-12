using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Seller;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminSellers
    {
        Task<List<SellerViewModel>> GetPaginateSellersAsync(int offset, int limit, string orderBy, string searchText);
    }
}
