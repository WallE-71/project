using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.ViewModels.Category;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminCategories
    {
        Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(int offset, int limit, string orderBy, string searchText);
        bool IsExistCategory(string categoryName, int recentCategoryId = 0);
        Task<List<TreeViewCategory>> GetAllCategoriesAsync();
        void BindSubCategories(TreeViewCategory category);
        Category FindByCategoryName(string categoryName);

        void LoadParent(Category category);
        int CountCategories();
    }
}
