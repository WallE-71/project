using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Category;

namespace ShoppingStore.Application.Services
{
    public class AdminCategories : IAdminCategories
    {
        private readonly IDataBaseContext _context;
        public AdminCategories(IDataBaseContext context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
        }

        public async Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(int offset, int limit, string orderBy, string searchText)
        {
            List<CategoryViewModel> categories = await _context.Categories
                                .GroupJoin(_context.Categories,
                                (cl => cl.ParentId),
                                (or => or.Id),
                                ((cl, or) => new { CategoryInfo = cl, ParentInfo = or }))
                                .SelectMany(p => p.ParentInfo.DefaultIfEmpty(), (x, y) => new { x.CategoryInfo, ParentInfo = y })
                                .OrderBy(orderBy)
                                .Skip(offset).Take(limit)
                                .Select(c => new CategoryViewModel
                                {
                                    Id = c.CategoryInfo.Id,
                                    Name = c.CategoryInfo.Name,
                                    ParentId = c.ParentInfo.Id,
                                    ParentName = c.ParentInfo.Name
                                }).AsNoTracking().ToListAsync();

            foreach (var item in categories)
                item.Row = ++offset;

            return categories;
        }

        public bool IsExistCategory(string categoryName, int recentCategoryId = 0)
        {
            if (recentCategoryId == 0)
                return _context.Categories.Any(c => c.Name.Trim().Replace(" ", "") == categoryName.Trim().Replace(" ", ""));
            else
            {
                var category = _context.Categories.Where(c => c.Name.Trim().Replace(" ", "") == categoryName.Trim().Replace(" ", "")).FirstOrDefault();
                if (category == null)
                    return false;
                else
                {
                    if (category.Id != recentCategoryId)
                        return true;
                    else
                        return false;
                }
            }
        }

        public async Task<List<TreeViewCategory>> GetAllCategoriesAsync()
        {
            var Categories = await (from c in _context.Categories
                                    where (c.ParentId == 0 || c.ParentId == null)
                                    select new TreeViewCategory { id = c.Id, title = c.Name }).ToListAsync();

            foreach (var item in Categories)
                BindSubCategories(item);

            return Categories;
        }

        public void BindSubCategories(TreeViewCategory category)
        {
            var SubCategories = (from c in _context.Categories
                                 where (c.ParentId == category.id)
                                 select new TreeViewCategory { id = c.Id, title = c.Name }).ToList();

            foreach (var item in SubCategories)
            {
                BindSubCategories(item);
                category.subs.Add(item);
            }
        }

        public Category FindByCategoryName(string categoryName) => _context.Categories
                                        .Where(c => c.Name == categoryName.TrimStart().TrimEnd()).FirstOrDefault();

        public void LoadParent(Category category) => _context.Entry<Category>(category).Reference(c => c.Parent).Load();
        public int CountCategories() => _context.Categories.Count();
    }
}
