using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels.City;

namespace ShoppingStore.Application.Services
{
    public class AdminProvinces : IAdminProvinces
    {
        private readonly IDataBaseContext _context;
        public AdminProvinces(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<CityProvinceViewModel>> GetPaginateProvincesAsync(int offset, int limit, string orderby, string searchText)
        {
            var viewModels = await _context.Provinces.Include(p => p.Cities)
                                .Where(p => p.RemoveTime == null && p.Name.Contains(searchText))
                                .OrderBy(orderby).Skip(offset).Take(limit)
                                .Select(p => new CityProvinceViewModel
                                {
                                    Id = p.Id,
                                    ProvinceName = p.Name,
                                    numberOfCities = p.Cities.Count,
                                    CityId = p.Cities.Where(p => p.ProvinceId == p.Id).FirstOrDefault().Id
                                }).ToListAsync();

            foreach (var item in viewModels)
                item.Row = ++offset;
            return viewModels;
        }

        public async Task<List<TreeViewProvince>> GetProvincesAsync()
        {
            return await (from p in _context.Provinces
                          where (p.RemoveTime == null)
                          select new TreeViewProvince { id = p.Id, title = p.Name }).ToListAsync();
        }
    }
}
