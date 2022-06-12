using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Order;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminOrders
    {
        Task<List<OrderViewModel>> GetPaginateOrdersAsync(int offset, int limit, string orderBy, string searchText);
        long TodaySaleAmount();
    }
}
