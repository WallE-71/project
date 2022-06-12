using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Seller;

namespace ShoppingStore.Application.Services
{
    public class SiteSellers : ISiteSellers
    {
        private readonly IDataBaseContext _context;
        public SiteSellers(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<SellerViewModel> GetSellerForProductAsync(string productId)
        {
            var viewModel = await (from s in _context.Sellers.Include(s => s.Ratings)
                                   join product in _context.Products on s.Id equals product.SellerId
                                   where (product.Id == productId)
                                   select (new SellerViewModel
                                   {
                                       Id = s.Id,
                                       Name = s.Name,
                                       InsertTime = s.InsertTime,
                                       IsComplete = s.IsComplete,
                                       RegisterDate = s.RegisterDate,
                                       PersianInsertTime = s.InsertTime.DateTimeEn2Fa("yyyy/MM/dd"),
                                       PersianRegisterDate = s.RegisterDate.DateTimeEn2Fa("yyyy/MM/dd"),
                                       Description = s.Description.Replace("<p>", "").Replace("</p>", ""),
                                       NumberOfProducts = s.Products.Where(p => p.SellerId == s.Id).Count(),
                                       UserVote = s.Ratings.FirstOrDefault(r => r.SellerId == s.Id).UserVote,
                                   })).AsNoTracking().FirstOrDefaultAsync();

            var image = await _context.FileStores.FirstOrDefaultAsync(f => f.SellerId == viewModel.Id);
            if (image != null)
                viewModel.SellerImage = image.ImageName;
            return viewModel;
        }
    }
}
