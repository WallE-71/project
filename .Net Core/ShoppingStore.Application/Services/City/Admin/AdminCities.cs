using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels.City;

namespace ShoppingStore.Application.Services
{
    public class AdminCities : IAdminCities
    {
        private readonly IDataBaseContext _context;
        public AdminCities(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<CityProvinceViewModel>> GetPaginateCitiesAsync(int offset, int limit, string orderby, string searchText, int cityId)
        {
            var viewModels = await _context.Cities.Include(c => c.Province)
                                .Where(c => (cityId == 0 || c.Id.Equals(cityId)) && c.Name.Contains(searchText))
                                .OrderBy(orderby).Skip(offset).Take(limit)
                                .Select(c => new CityProvinceViewModel
                                {
                                    Id = c.Id,
                                    CityName = c.Name,
                                    ProvinceName = c.Province.Name,
                                    numberOfUser = _context.Users.Where(u => u.CityId == c.Id).Count(),
                                }).ToListAsync();

            foreach (var item in viewModels)
                item.Row = ++offset;
            return viewModels;
        }

        public async Task<List<TreeViewCity>> GetCitiesAsync()
        {
            return await (from c in _context.Cities
                          where (c.RemoveTime == null)
                          select new TreeViewCity { id = c.Id, title = c.Name }).ToListAsync();
        }
    }
}
