using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.ViewModels;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminColors
    {
        Task<List<BaseViewModel<int>>> GetPaginateColorsAsync(int offset, int limit, string orderby, string searchText);
        bool IsExistColor(string name, int recentId = 0);
        Task<ICollection<ProductColor>> InsertProductColors(string[] colors, string primaryColor, string productId = null);
    }
}
