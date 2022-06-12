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
    public class AdminMaterials : IAdminMaterials
    {
        private readonly IDataBaseContext _context;
        public AdminMaterials(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<BaseViewModel<int>>> GetPaginateMaterialsAsync(int offset, int limit, string orderby, string searchText)
        {
            var materials = await _context.Materials.Include(c => c.ProductMaterials)
                                .Where(c => c.Name.Contains(searchText))
                                .OrderBy(orderby).Skip(offset).Take(limit)
                                .Select(c => new BaseViewModel<int>
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    Description = c.ProductMaterials.Where(pc => pc.MaterialId == c.Id).Count().ToString().En2Fa()
                                }).AsNoTracking().ToListAsync();

            foreach (var item in materials)
                item.Row = ++offset;
            return materials;
        }

        public bool IsExistMaterial(string name, int recentId = 0)
        {
            if (recentId == 0)
                return _context.Materials.Any(c => c.Name.Trim().Replace(" ", "") == name.Trim().Replace(" ", ""));
            else
            {
                var material = _context.Materials.Where(c => c.Name.Trim().Replace(" ", "") == name.Trim().Replace(" ", "")).FirstOrDefault();
                if (material == null)
                    return false;
                else
                {
                    if (material.Id != recentId)
                        return true;
                    else
                        return false;
                }
            }
        }

        public async Task<ICollection<ProductMaterial>> InsertProductMaterials(string[] materials, string productId = null)
        {
            var productMaterials = new List<ProductMaterial>();
            var allMaterials = await _context.Materials.ToListAsync();
            productMaterials.AddRange(allMaterials.Where(n => materials.Contains(n.Name)).Select(m => new ProductMaterial { ProductId = productId, MaterialId = m.Id }).ToList());
            var newMaterials = materials.Where(i => !allMaterials.Select(m => m.Name).Contains(i)).ToList();
            foreach (var name in newMaterials)
            {
                if (!IsExistMaterial(name))
                {
                    var _allmaterials = await _context.Materials.ToListAsync();
                    var maxId = _allmaterials.Count() == 0 ? 1 : _allmaterials.OrderByDescending(m => m.Id).First().Id + 1;
                    await _context.Materials.AddAsync(new Domain.Entities.Material { Name = name, Id = maxId, InsertTime = DateTime.Now });
                    productMaterials.Add(new ProductMaterial { ProductId = productId, MaterialId = maxId });
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var material = await _context.Materials.FirstOrDefaultAsync(c => c.Name == name);
                    productMaterials.Add(new ProductMaterial { ProductId = productId, MaterialId = material.Id });
                }
            }
            return productMaterials;
        }
    }
}
