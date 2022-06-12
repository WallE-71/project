using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.ViewModels;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminMaterials
    {
        Task<List<BaseViewModel<int>>> GetPaginateMaterialsAsync(int offset, int limit, string orderby, string searchText);
        bool IsExistMaterial(string name, int recentId = 0);
        Task<ICollection<ProductMaterial>> InsertProductMaterials(string[] materials, string productId = null);
    }
}
