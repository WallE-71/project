using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingStore.Persistence;
using ShoppingStore.Infrastructure.Api;
using ShoppingStore.Application.Services;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Infrastructure.Contract;
using ShoppingStore.Application.ViewModels.Settings;
using ShoppingStore.Infrastructure. Contracts;

namespace ShoppingStore.Infrastructure.Extentions
{
    public static class AddCustomServicesExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataBaseContext>(options => options.UseSqlServer(
                   configuration.GetConnectionString("SqlServer"),
                   b => b.MigrationsAssembly(typeof(DataBaseContext).Assembly.FullName)));

            services.AddScoped<IDataBaseContext, DataBaseContext>();
            services.AddHealthChecks().AddDbContextCheck<DataBaseContext>();
            services.Configure<SiteSettings>(configuration.GetSection(nameof(SiteSettings)));
            services.AddScoped<IDataBaseContext>(provider => provider.GetRequiredService<DataBaseContext>());

            //----------------------------------------------
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IProductService, ProductService>();

            services.AddTransient<IJwtService, JwtService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();
            //services.AddTransient<SendWeeklyNewsLetter>();


            //services.AddTransient(typeof(IBaseService<>), typeof(BaseService<>));
            //services.AddScoped<IAdminUsers, AdminUsers>();

            //services.AddScoped<ISellerFacade, SellerFacade>();

            //services.AddScoped<ISiteOrders, SiteOrders>();
            //services.AddScoped<IAdminOrders, AdminOrders>();

            //services.AddScoped<IProductService, ProductService>();

            //services.AddScoped<ISiteComments, SiteComments>();
            //services.AddScoped<IAdminComments, AdminComments>();
            //services.AddScoped<IInsertCommentUser, InsertCommentUser>();

            //services.AddScoped<ISiteCarts, SiteCarts>();
            //services.AddScoped<IAdminCarts, AdminCarts>();
            //services.AddScoped<IManageShoppingCart, ManageShoppingCart>();

            //services.AddScoped<ISitePayments, SitePayments>();
            //services.AddScoped<IAdminPayments, AdminPayments>();

            //services.AddScoped<ISiteCategories, SiteCategories>();
            //services.AddScoped<IAdminCategories, AdminCategories>();

            //services.AddScoped<IAdminNewsletters, AdminNewsletters>();
            //services.AddScoped<IInsertNewsletter, InsertNewsletter>();

            //services.AddScoped<IAdminMessageUsers, AdminMessageUsers>();
            //services.AddScoped<IInsertMassageOfUser, InsertMassageOfUser>();

            //services.AddScoped<IAdminCreditCarts, AdminCreditCarts>();
            //services.AddScoped<IManageCreditCartUser, ManageCreditCartUser>();

            //services.AddScoped<IInsertLike, InsertLike>();
            //services.AddScoped<IAdminFiles, AdminFiles>();
            //services.AddScoped<IAdminBrands, AdminBrands>();
            //services.AddScoped<IAdminCities, AdminCities>();
            //services.AddScoped<IAdminColors, AdminColors>();
            //services.AddScoped<IAdminMaterials, AdminMaterials>();
            //services.AddScoped<IAdminProvinces, AdminProvinces>();
            //services.AddScoped<IInsertRateSaller, InsertRateSaller>();

            return services;
        }
    }
}
