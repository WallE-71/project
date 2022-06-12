using System.Threading.Tasks;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.ViewModels;
using static ShoppingStore.Application.ViewModels.RequestPayViewModel;

namespace ShoppingStore.Application.Contracts
{
    public interface ISitePayments
    {
        Task<ResultViewModel<ResultRequestPay>> AddRequestPayAsync(int amount, int userId, TransportType transportType, string discountCode);
        Task<ResultViewModel<ItemRequestPay>> GetRequestPayAsync(string requestPayId);
    }
}
