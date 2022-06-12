using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Category;

namespace ShoppingStore.Application.Services
{
    public class SiteCategories : ISiteCategories
    {
        private readonly IDataBaseContext _context;
        private readonly IAdminCategories _adminCategories;
        public SiteCategories(IDataBaseContext context, IAdminCategories adminCategories)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
            _adminCategories = adminCategories;
            _adminCategories.CheckArgumentIsNull(nameof(_adminCategories));
        }

        public async Task<List<TreeViewCategory>> GetSubCategoriesByName(string parentName)
        {
            var subCategories = await (from c in _context.Categories.Include(c => c.SubCategories)
                                       .Where(n => n.Name == parentName)
                                       select (new TreeViewCategory
                                       {
                                           id = c.Id,
                                           title = c.Name
                                       })).AsNoTracking().ToListAsync();

            var _subCategories = new List<TreeViewCategory>();
            var treeViewCategories = new List<TreeViewCategory>();
            foreach (var item in subCategories)
            {
                _adminCategories.BindSubCategories(item);
                var viewModel = new TreeViewCategory();
                var treeView = subCategories.FirstOrDefault(c => c.id == item.id);
                viewModel.title = treeView.title;
                if (item.subs.Count() != 0)
                {
                    foreach (var sub in item.subs)
                    {
                        var subCategory = new TreeViewCategory();
                        subCategory.title = sub.title;
                        _subCategories.Add(subCategory);
                    }
                }
                viewModel.subs = _subCategories;
                treeViewCategories.Add(viewModel);
            }
            return treeViewCategories;
        }
    }
}
