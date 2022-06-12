using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels.Slider;

namespace ShoppingStore.Application.Services
{
    public class SiteSliders : ISiteSliders
    {
        private readonly IDataBaseContext _context;
        public SiteSliders(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<SliderViewModel>> GetSlidersAsync()
        {
            return await _context.Sliders.Include(s => s.FileStores)
                                        .Where(s => s.RemoveTime == null)
                                        .Select(s => new SliderViewModel
                                        {
                                            Id = s.Id,
                                            Url = s.Url,
                                            InsertTime = s.InsertTime,
                                            TypeOfSlider = s.TypeOfSlider,
                                            ImageLocation = s.ImageLocation,
                                            ImageFile = s.FileStores.FirstOrDefault(f => f.SliderId == s.Id).ImageName,
                                            ImageFiles = s.FileStores.Where(f => f.SliderId == s.Id).Select(s => s.ImageName).ToList()
                                        }).AsNoTracking().ToListAsync();
        }
    }
}
