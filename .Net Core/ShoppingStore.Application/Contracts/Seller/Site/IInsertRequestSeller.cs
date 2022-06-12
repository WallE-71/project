using System.Threading.Tasks;
using ShoppingStore.Application.ViewModels.Api.Seller;

namespace ShoppingStore.Application.Contracts
{
    public interface IInsertRequestSeller
    {
        Task<int> RequestRegister(SellerDto dto);
    }
}
