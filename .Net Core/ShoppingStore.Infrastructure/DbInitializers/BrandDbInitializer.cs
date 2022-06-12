using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingStore.Persistence;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Infrastructure.Contracts;

namespace ShoppingStore.Infrastructure.DbInitializers
{
    public class BrandDbInitializer : IBrandDbInitializer
    {
        private IDataBaseContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BrandDbInitializer> _logger;
        public BrandDbInitializer(IDataBaseContext context, IServiceScopeFactory scopeFactory, ILogger<BrandDbInitializer> logger)
        {
            _logger = logger;
            _context = context;
            _scopeFactory = scopeFactory;
        }

        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataBaseContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var dbInitializer = serviceScope.ServiceProvider.GetService<IBrandDbInitializer>();
                var result = dbInitializer.SeedDatabaseAsync().Result;

                if (result == IdentityResult.Failed())
                    throw new InvalidOperationException(result.DumpErrors());
            }
        }

        public async Task<IdentityResult> SeedDatabaseAsync()
        {
            var thisMethodName = nameof(SeedDatabaseAsync);
            var brand = _context.Brands.Where(s => s.Id == 1).FirstOrDefault();
            if (brand != null)
            {
                _logger.LogInformation($"{thisMethodName}: this Seller already exists.");
                return IdentityResult.Success;
            }

            _context.Brands.AddRange(
            brand = new Brand
            {
                Id = 1,
                Name = "پرژک",
                InsertTime = DateTime.Now,
            });
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }
    }
}
