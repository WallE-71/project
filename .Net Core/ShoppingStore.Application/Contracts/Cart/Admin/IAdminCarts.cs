using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Cart;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminCarts
    {
        Task<List<CartViewModel>> GetPaginateCartsAsync(int offset, int limit, string orderBy, string searchText);
        Task<List<CartItemViewModel>> GetPaginateCartItemsAsync(int offset, int limit, string orderBy, string searchText, int cartId);
    }
}
