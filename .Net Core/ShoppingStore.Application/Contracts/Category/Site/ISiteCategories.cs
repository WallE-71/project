using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Category;

namespace ShoppingStore.Application.Contracts
{
    public interface ISiteCategories
    {
        Task<List<TreeViewCategory>> GetSubCategoriesByName(string parentName);
    }
}
