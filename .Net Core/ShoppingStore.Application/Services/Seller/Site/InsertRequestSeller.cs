using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Api.Seller;

namespace ShoppingStore.Application.Services
{
    public class InsertRequestSeller : IInsertRequestSeller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IDataBaseContext _context;
        public InsertRequestSeller(IWebHostEnvironment env, IDataBaseContext context)
        {
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
        }

        public virtual async Task<int> RequestRegister(SellerDto dto)
        {
            var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.NationalId == dto.NationalId || s.PhonNumber == dto.PhoneNumber);
            if (seller != null) return 0;

            int maxId;
            var sellers = await _context.Sellers.AsNoTracking().ToListAsync();
            if (sellers.Count() != 0)
                maxId = sellers.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
            else
                maxId = 1;
            seller = new Seller
            {
                Id = maxId,
                Name = dto.Name,
                Email = dto.Email,
                Brand = dto.Brand,
                SurName = dto.SurName,
                WebSite = dto.WebSite,
                NationalId = dto.NationalId,
                PhonNumber = dto.PhoneNumber,
                Description = dto.Description,
                ActivityType = (ActivityType)dto.ActivityType,
                InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
            };
            await _context.Sellers.AddAsync(seller);
            await _context.SaveChangesAsync();

            if (dto.Store.HasValue())
            {
                var newStore = new Store
                {
                    Id = maxId,
                    Name = dto.Store,
                    SellerId = maxId,
                    Address = dto.Address,
                    TelNumber = dto.TelNumber,
                    InsertTime = DateTime.Now,
                    PostalCode = dto.PostalCode,
                    Description = dto.SampleProduct,
                    EstablishmentDate = dto.EstablishmentDate.ConvertShamsiToMiladi(),
                };
                await _context.Stores.AddAsync(newStore);
            }
            await _context.SaveChangesAsync();
            return seller.Id;
        }
    }
}
