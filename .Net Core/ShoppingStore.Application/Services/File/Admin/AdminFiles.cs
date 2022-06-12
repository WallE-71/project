using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.File;

namespace ShoppingStore.Application.Services
{
    public class AdminFiles : IAdminFiles
    {
        private readonly IDataBaseContext _context;
        public AdminFiles(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task InsertImagesAsync(string productId, int? sellerId, int? sliderId, int? userId, List<string> nameImages)
        {
            foreach (var item in nameImages)
            {
                var file = new FileStore
                {
                    ImageName = item,
                    UserId = userId,
                    SliderId = sliderId,
                    SellerId = sellerId,
                    ProductId = productId,
                    InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                };
                await _context.FileStores.AddAsync(file);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateImagesAsync(string productId, int? sellerId, int? sliderId, int? userId, List<string> nameImages)
        {
            foreach (var item in nameImages)
            {
                var file = new FileStore
                {
                    ImageName = item,
                    UserId = userId,
                    SliderId = sliderId,
                    SellerId = sellerId,
                    ProductId = productId,
                    UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                };
                _context.FileStores.Update(file);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<FileViewModel>> GetPaginateFilesAsync(int offset, int limit, string Orderby, string searchText)
        {
            var files = await _context.FileStores
                                   .OrderBy(Orderby).Skip(offset).Take(limit)
                                   .Select(f => new FileViewModel
                                   {
                                       ImageName = f.ImageName,
                                       SellerName = _context.Sellers.FirstOrDefault(s => s.Id == f.SellerId).Name,
                                       ProductName = _context.Products.FirstOrDefault(p => p.Id == f.ProductId).Name,
                                   }).AsNoTracking().ToListAsync();

            foreach (var item in files)
                item.Row = ++offset;
            return files;
        }

        public async Task<string> FindImageAsync(string productId, int? sellerId, int? sliderId, int? userId)
        {
            if (productId != null)
            {
                var fileStore = await _context.FileStores.FirstOrDefaultAsync(f => f.ProductId == productId);
                if (fileStore != null)
                {
                    var check = !fileStore.ImageName.Contains($"product-{productId}-");
                    if (check)
                        return fileStore.ImageName.Trim().Replace(" ", "");
                }
            }
            if (sellerId != 0 && sellerId != null)
            {
                var fileStore = await _context.FileStores.FirstOrDefaultAsync(f => f.SellerId == sellerId);
                if (fileStore != null)
                {
                    var check = !fileStore.ImageName.Contains($"seller-{sellerId}-");
                    if (check)
                        return fileStore.ImageName.Trim().Replace(" ", "");
                }
            }
            if (sliderId != 0 && sliderId != null)
            {
                var fileStore = await _context.FileStores.FirstOrDefaultAsync(f => f.SliderId == sliderId);
                if (fileStore != null)
                {
                    var check = !fileStore.ImageName.Contains($"slider-{sliderId}-");
                    if (check)
                        return fileStore.ImageName.Trim().Replace(" ", "");
                }
            }
            if (userId != 0 && userId != null)
            {
                var fileStore = await _context.FileStores.FirstOrDefaultAsync(f => f.UserId == userId);
                if (fileStore != null)
                {
                    var check = !fileStore.ImageName.Contains($"avatars-{userId}-");
                    if (check)
                        return fileStore.ImageName.Trim().Replace(" ", "");
                }
            }
            return null;
        }

        public async Task<List<string>> GetImagesAsync(string productId, int? sellerId, int? sliderId)
        {
            var images = new List<string>();
            if (productId != null)
            {
                var fileStores = await _context.FileStores.Where(f => f.ProductId == productId).ToListAsync();
                foreach (var fileStore in fileStores)
                    images.Add(fileStore.ImageName.Trim().Replace(" ", ""));
            }
            if (sellerId != 0 && sellerId != null)
            {
                var fileStores = await _context.FileStores.Where(f => f.SellerId == sellerId).ToListAsync();
                foreach (var fileStore in fileStores)
                    images.Add(fileStore.ImageName.Trim().Replace(" ", ""));
            }
            if (sliderId != 0 && sliderId != null)
            {
                var fileStores = await _context.FileStores.Where(f => f.SliderId == sliderId).ToListAsync();
                foreach (var fileStore in fileStores)
                    images.Add(fileStore.ImageName.Trim().Replace(" ", ""));
            }
            if (images.Count != 0)
                return images;
            return new List<string>();
        }
    }
}
