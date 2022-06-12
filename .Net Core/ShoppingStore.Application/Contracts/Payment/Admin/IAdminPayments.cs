using System.Threading.Tasks;
using System.Collections.Generic;
using static ShoppingStore.Application.ViewModels.RequestPayViewModel;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminPayments
    {
        Task<List<ItemRequestPay>> GetPaginateRequestPaysAsync(int offset, int limit, bool? sortAsc, string searchText);
    }
}
