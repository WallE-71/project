using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Slider;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminSliders
    {
        Task<List<SliderViewModel>> GetPaginateSlidersAsync(int offset, int limit, string orderby, string searchText);
    }
}
