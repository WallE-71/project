using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;

namespace ShoppingStore.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IDataBaseContext _context;
        public ProductService(IDataBaseContext context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
        }

        public async Task<string> GetMaxDiscountTypeInCategory()
        {
            var discount = await (from d in _context.Discounts
                                  .Where(d => d.Percent != 0)
                                  select (new
                                  {
                                      Percent = d.Percent,
                                      EndDate = d.EndDate,
                                      StartDate = d.StartDate,
                                      DiscountDay = d.DiscountDay,
                                      DiscountCode = d.DiscountCode
                                  })).OrderByDescending(d => d.Percent).AsNoTracking().FirstOrDefaultAsync();

            var date = DateTime.Now.Date;
            var endDate = discount.EndDate?.Date;
            var startDate = discount.StartDate.Value.Date;

            return
                discount.DiscountCode != null ? "کد تخفیف" :
                discount.DiscountDay != null && DateTime.Now >= discount.StartDate ? "تخفیف ویژه" :
                endDate <= startDate.AddMonths(3) && endDate > startDate.AddMonths(1) ? "فصلی" :
                endDate <= startDate.AddMonths(1) && endDate > startDate.AddDays(7) ? "ماهانه" :
                endDate <= startDate.AddDays(7) ? "هفتگی" :
                discount.StartDate <= date && discount.EndDate <= date.EndOfDay() ? "روزانه" : null;
        }


        public async Task<List<string>> GetProductImagesAsync(string productId, int offset, int limit)
        {
            var images = new List<string>();
            var fileStores = await _context.FileStores.Where(f => f.ProductId == productId)
                .OrderByDescending(f => f.InsertTime).Skip(offset).Take(limit).AsNoTracking().ToListAsync();
            foreach (var fileStore in fileStores)
                images.Add(fileStore.ImageName);
            return images;
        }

        public async Task<int?> DiscountManagerAsync(string productId, bool save = false)
        {
            var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.ProductId == productId);
            if (discount == null) return null;
            if (discount.EndDate < DateTime.Now || discount.EndDate == null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
                return null;
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return null;

            if (product.Price <= 0) return 0;
            if (product.States != ProductState.Ready) return 0;
            var actionDiscount = product.Price * Convert.ToInt32(discount.Percent) / 100;
            if (save && discount.EndDate > DateTime.Now)
            {
                product.Price = actionDiscount;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            return actionDiscount;
        }

        public async Task ProductStateManagerAsync(string productId)
        {
            var save = false;
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            //var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.Id == product.SellerId);
            //if (seller == null)
            //{
            //    product.States = ProductState.Demo;
            //    save = true;
            //}
            if (product.InsertTime <= DateTime.Now && product.States == ProductState.CoomingSoon)
            {
                product.States = ProductState.Demo;
                save = true;
            }
            if (product.InsertTime > DateTime.Now && product.States != ProductState.CoomingSoon)
            {
                product.States = ProductState.CoomingSoon;
                save = true;
            }
            if (product.Stock <= 0)
            {
                product.Stock = 0;
                product.States = ProductState.EndOfStock;
                save = true;
            }
            if (product.ExpirationDate <= DateTime.Now)
            {
                product.States = ProductState.ExpirationEnd;
                save = true;
            }
            if (save)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GetProductImage(string productId) => await _context.FileStores
                                    .FirstOrDefaultAsync(f => f.ProductId == productId) == null ? null : _context.FileStores
                                    .FirstOrDefaultAsync(f => f.ProductId == productId).Result.ImageName;
    }

    public static class ProductExtension
    {
        public static string GetDiscountName(this Discount discount)
        {
            if (discount == null) return "ندارد";
            var date = DateTime.Now.Date;
            var endDate = discount.EndDate?.Date;
            return discount.DiscountCode != null ? "کد تخفیف" :
                        endDate <= date.AddYears(1) && endDate > date.AddMonths(3) ? "تخفیف طولانی" :
                        endDate <= date.AddMonths(3) && endDate > date.AddMonths(1) ? "فصلی" :
                        endDate <= date.AddMonths(1) && endDate > date.AddDays(7) ? "ماهانه" :
                        endDate <= date.AddDays(7) && endDate > date.EndOfDay() ? "هفتگی" :
                        discount.DiscountDay != null ? "ویژه" : "ندارد";
            //endDate <= date.AddHours(6) ? "فوری";
        }

        public static string GetProductState(this ProductState state)
        {
            if (state == 0)
                return "بزودی";
            return state == ProductState.Demo ? "دمو" :
                                        state == ProductState.Stockroom ? "در انبار" :
                                        state == ProductState.Ready ? "آماده عرضه" :
                                        state == ProductState.EndOfStock ? "موجود نیست" :
                                        state == ProductState.ExpirationEnd ? "منقضی شده" :
                                        state == ProductState.Returned ? "مرجوع شده" :
                                        state == ProductState.Corrupted ? "آسیب دیده" : "بزودی";
        }
    }
}
