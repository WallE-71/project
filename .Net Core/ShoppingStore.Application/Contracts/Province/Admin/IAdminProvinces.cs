using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.City;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminProvinces
    {
        Task<List<TreeViewProvince>> GetProvincesAsync();
        Task<List<CityProvinceViewModel>> GetPaginateProvincesAsync(int offset, int limit, string orderby, string searchText);
    }
}
