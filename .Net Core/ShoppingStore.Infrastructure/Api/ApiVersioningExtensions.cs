using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace ShoppingStore.Infrastructure.Api
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection ApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader(),
                    new HeaderApiVersionReader("api-version"));
            });
            return services;
        }
    }
}
