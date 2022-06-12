using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.CreditCart;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminCreditCarts
    {
        Task<List<CreditCartViewModel>> GetPaginateCreditCartsAsync(int offset, int limit, string orderBy, string searchText);
    }
}
