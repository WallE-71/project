using System;
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
    public class CategoriesDbInitializer : ICategoriesDbInitializer
    {
        private readonly IUnitOfWork _uw;
        private readonly IDataBaseContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CategoriesDbInitializer> _logger;
        public CategoriesDbInitializer(
            IUnitOfWork uw,
            IDataBaseContext context,
            IServiceScopeFactory scopeFactory,
            ILogger<CategoriesDbInitializer> logger)
        {
            _uw = uw;
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
                var dbInitializer = serviceScope.ServiceProvider.GetService<ICategoriesDbInitializer>();
                var result = dbInitializer.SeedDatabaseAsync().Result;

                if (result == IdentityResult.Failed())
                    throw new InvalidOperationException(result.DumpErrors());
            }
        }

        public async Task<IdentityResult> SeedDatabaseAsync()
        {
            try
            {
                var thisMethodName = nameof(SeedDatabaseAsync);
                var category1 = _uw.AdminCategories.FindByCategoryName("لبنیات");
                var category2 = _uw.AdminCategories.FindByCategoryName("خواربار و نان");
                var category3 = _uw.AdminCategories.FindByCategoryName("دستمال و شوینده");
                var category4 = _uw.AdminCategories.FindByCategoryName("تنقلات");
                var category5 = _uw.AdminCategories.FindByCategoryName("نوشیدنی");
                var category6 = _uw.AdminCategories.FindByCategoryName("مواد پروتئینی");
                var category7 = _uw.AdminCategories.FindByCategoryName("آرایشی و بهداشتی");
                var category8 = _uw.AdminCategories.FindByCategoryName("چاشنی و افزودنی");
                var category9 = _uw.AdminCategories.FindByCategoryName("میوه و سبزیجات تازه");
                var category10 = _uw.AdminCategories.FindByCategoryName("کنسرو و غذای آماده");
                var category11 = _uw.AdminCategories.FindByCategoryName("صبحانه");
                var category12 = _uw.AdminCategories.FindByCategoryName("خشکبار، دسر و شیرینی");
                var category13 = _uw.AdminCategories.FindByCategoryName("خانه و سبک زندگی");
                var category14 = _uw.AdminCategories.FindByCategoryName("کودک و نوزاد");
                var category15 = _uw.AdminCategories.FindByCategoryName("مد و پوشاک");
                if (category1 != null && category2 != null && category3 != null && category4 != null && category5 != null && category6 != null && category7 != null && category8 != null && category9 != null && category10 != null && category11 != null && category12 != null && category13 != null && category14 != null && category15 != null)
                {
                    _logger.LogInformation($"{thisMethodName}: this Category already exists.");
                    return IdentityResult.Success;
                }

                _context.Categories.AddRange(
                new Category
                {
                    Id = 1,
                    Name = "لبنیات",
                    ParentId = null,
                },
                new Category
                {
                    Id = 2,
                    Name = "خواربار و نان",
                    ParentId = null,
                },
                new Category
                {
                    Id = 3,
                    Name = "دستمال و شوینده",
                    ParentId = null,
                },
                new Category
                {
                    Id = 4,
                    Name = "تنقلات",
                    ParentId = null,
                },
                new Category
                {
                    Id = 5,
                    Name = "نوشیدنی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 6,
                    Name = "مواد پروتئینی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 7,
                    Name = "آرایشی و بهداشتی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 8,
                    Name = "چاشنی و افزودنی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 9,
                    Name = "میوه و سبزیجات تازه",
                    ParentId = null,
                },
                new Category
                {
                    Id = 10,
                    Name = "کنسرو و غذای آماده",
                    ParentId = null,
                },
                new Category
                {
                    Id = 11,
                    Name = "صبحانه",
                    ParentId = null,
                },
                new Category
                {
                    Id = 12,
                    Name = "خشکبار، دسر و شیرینی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 13,
                    Name = "خانه و سبک زندگی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 14,
                    Name = "کودک و نوزاد",
                    ParentId = null,
                },
                new Category
                {
                    Id = 15,
                    Name = "مد و پوشاک",
                    ParentId = null,
                });

                await _context.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch(Exception) { }
            return new IdentityResult();
        }
    }
}
