using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.City;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminCities
    {
        Task<List<TreeViewCity>> GetCitiesAsync();
        Task<List<CityProvinceViewModel>> GetPaginateCitiesAsync(int offset, int limit, string orderby, string searchText, int cityId);
    }
}
