using System.Threading.Tasks;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.ViewModels;
using ShoppingStore.Application.ViewModels.Cart;

namespace ShoppingStore.Application.Contracts
{
    public interface ISiteCarts
    {
        Task<CartViewModel> GetCartAsync(int userId, string browserId);
        Task<ResultViewModel<CartItem>> AddToCartAsync(string productId, string browserId, string color);
        Task<ResultViewModel> Increase(int cartItemId);
        Task<ResultViewModel> Decrease(int cartItemId);
        Task<ResultViewModel> RemoveFromCart(string productId, string browserId);
        Task<ResultViewModel> RemoveAllFromCart(string browserId);
    }
}
