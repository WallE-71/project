using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminBrands
    {
        Task<List<BaseViewModel<int>>> GetPaginateBrandsAsync(int offset, int limit, string Orderby, string searchText);
        bool IsExistBrand(string name, int recentId = 0);
        Task<int> InsertProductBrand(string brandName);
    }
}
