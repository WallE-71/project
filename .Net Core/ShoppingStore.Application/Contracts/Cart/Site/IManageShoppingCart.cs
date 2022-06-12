using System.Threading.Tasks;
using ShoppingStore.Application.ViewModels.Cart;

namespace ShoppingStore.Application.Contracts
{
    public interface IManageShoppingCart
    {
        Task<CartViewModel> ManageCart(CartViewModel result, string browserId);
    }
}
