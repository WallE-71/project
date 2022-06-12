using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Slider;

namespace ShoppingStore.Application.Contracts
{
    public interface ISiteSliders
    {
        Task<List<SliderViewModel>> GetSlidersAsync();
    }
}
