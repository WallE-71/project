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
    public class AdminBrands : IAdminBrands
    {
        private readonly IDataBaseContext _context;
        public AdminBrands(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<BaseViewModel<int>>> GetPaginateBrandsAsync(int offset, int limit, string Orderby, string searchText)
        {
            var brands = await _context.Brands.Where(c =>c.RemoveTime == null && c.Name.Contains(searchText))
                                   .OrderBy(Orderby).Skip(offset).Take(limit)
                                   .Select(b => new BaseViewModel<int>
                                   {
                                       Id = b.Id,
                                       Name = b.Name,
                                       Description = _context.Products.Where(p => p.BrandId == b.Id).Count().ToString().En2Fa(),
                                   }).AsNoTracking().ToListAsync();

            foreach (var item in brands)
                item.Row = ++offset;
            return brands;
        }

        public bool IsExistBrand(string name, int recentId = 0)
        {
            if (recentId == 0)
                return _context.Brands.Any(c => c.Name.Trim().Replace(" ", "") == name.Trim().Replace(" ", ""));
            else
            {
                var brand = _context.Brands.Where(c => c.Name.Trim().Replace(" ", "") == name.Trim().Replace(" ", "")).FirstOrDefault();
                if (brand == null) return false;
                else
                {
                    if (brand.Id != recentId) return true;
                    else return false;
                }
            }
        }

        public async Task<int> InsertProductBrand(string brandName)
        {
            int id;
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == brandName);
            if (brand != null)
            {
                id = brand.Id;
                brand.Name = brandName;
                brand.InsertTime = DateTime.Now.Date;
                _context.Brands.Update(brand);
            }
            else
            {
                var allBrands = _context.Brands.Where(b => b.RemoveTime == null).ToList();
                if (allBrands.Count() == 0)
                    id = 1;
                else
                    id = allBrands.OrderByDescending(c => c.Id).First().Id + 1;
                await _context.Brands.AddAsync(new Brand { Name = brandName, Id = id });
            }
            await _context.SaveChangesAsync();
            return id;
        }
    }
}
