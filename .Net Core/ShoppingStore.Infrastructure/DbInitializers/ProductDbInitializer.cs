using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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
    public class ProductDbInitializer : IProductDbInitializer
    {
        private IDataBaseContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProductDbInitializer> _logger;
        public ProductDbInitializer(IDataBaseContext context, IServiceScopeFactory scopeFactory, ILogger<ProductDbInitializer> logger)
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
                var dbInitializer = serviceScope.ServiceProvider.GetService<IProductDbInitializer>();
                var result = dbInitializer.SeedDatabaseAsync().Result;

                if (result == IdentityResult.Failed())
                    throw new InvalidOperationException(result.DumpErrors());
            }
        }

        public async Task<IdentityResult> SeedDatabaseAsync()
        {
            var product = new Product();
            var thisMethodName = nameof(SeedDatabaseAsync);
            product = await _context.Products.FirstOrDefaultAsync(p => p.Name == "شامپو انار پرژک 450 گرمی" && p.SellerId == 1);
            if (product != null)
            {
                _logger.LogInformation($"{thisMethodName}: this Product already exists.");
                return IdentityResult.Success;
            }

            var productId = StringExtensions.GenerateId(10);
            var productCategories = new List<ProductCategory>();
            productCategories.Add(new ProductCategory { CategoryId = 1, ProductId = productId });
            _context.Products.AddRange(
            product = new Product
            {
                BrandId = 1,
                SellerId = 1,
                Stock = 1000,
                Price = 20000,
                Id = productId,
                IsPrefered = false,
                InsertTime = DateTime.Now,
                States = ProductState.Demo,
                Name = "شامپو انار پرژک 450 گرمی",
                Description = "<p>Nice Product</p>",
                ProductCategories = productCategories,
                ExpirationDate = DateTime.Today.AddYears(1)
            });
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return IdentityResult.Failed();
        }
    }
}
