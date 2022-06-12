using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels;
using ShoppingStore.Domain.Common.Extensions;

namespace ShoppingStore.Application.Services
{
    public class AdminColors : IAdminColors
    {
        private readonly IDataBaseContext _context;
        public AdminColors(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<BaseViewModel<int>>> GetPaginateColorsAsync(int offset, int limit, string orderby, string searchText)
        {
            var viewModels = await _context.Colors.Include(c => c.ProductColors)
                                .Where(c => c.Name.Contains(searchText))
                                .OrderBy(orderby).Skip(offset).Take(limit)
                                .Select(c => new BaseViewModel<int>
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    Description = c.ProductColors.Where(pc => pc.ColorId == c.Id).Count().ToString().En2Fa()
                                }).AsNoTracking().ToListAsync();

            foreach (var item in viewModels)
                item.Row = ++offset;
            return viewModels;
        }

        public bool IsExistColor(string name, int recentId = 0)
        {
            if (recentId == 0)
                return _context.Colors.Any(c => c.Name.Trim().Replace(" ", "") == name.Trim().Replace(" ", ""));
            else
            {
                var color = _context.Colors.Where(c => c.Name.Trim().Replace(" ", "") == name.Trim().Replace(" ", "")).FirstOrDefault();
                if (color == null)
                    return false;
                else
                {
                    if (color.Id != recentId)
                        return true;
                    else
                        return false;
                }
            }
        }

        public async Task<ICollection<ProductColor>> InsertProductColors(string[] colors, string primaryColor, string productId = null)
        {
            var newColors = new List<string>();
            var productColors = new List<ProductColor>();
            var allColors = await _context.Colors.ToListAsync();

            foreach (var item in colors)
            {
                if (!item.Contains(primaryColor))
                    productColors.Add(allColors.Where(n => item == n.Name).Select(c => new ProductColor { ProductId = productId, ColorId = c.Id }).FirstOrDefault());
            }
            newColors = colors.Where(i => !allColors.Select(m => m.Name).Contains(i)).ToList();
            if (primaryColor.HasValue())
                newColors.Add(primaryColor);

            foreach (var hexColor in newColors)
            {
                if (!IsExistColor(hexColor))
                {
                    var _allColor = await _context.Colors.ToListAsync();
                    var maxId = _allColor.Count() == 0 ? 1 : _allColor.OrderByDescending(m => m.Id).First().Id + 1;
                    await _context.Colors.AddAsync(new Color { Id = maxId, Name = hexColor, InsertTime = DateTime.Now });
                    productColors.Add(new ProductColor { ProductId = productId, ColorId = maxId });
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == hexColor);
                    productColors.Add(new ProductColor { ProductId = productId, ColorId = color.Id });
                }
            }
            return productColors;
        }
    }
}
