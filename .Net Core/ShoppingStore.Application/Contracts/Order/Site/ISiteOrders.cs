using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels;
using ShoppingStore.Application.ViewModels.Order;

namespace ShoppingStore.Application.Contracts
{
    public interface ISiteOrders
    {
        Task<ResultViewModel> RequestAddNewOrderAsync(RequestAddNewOrder request);
        Task<ResultViewModel<List<UserOrderViewModel>>> GetUserOrdersAsync(int userId, int offset, int limit);
        ResultViewModel RemoveOrder(int orderId, int userId);       
    }
}
