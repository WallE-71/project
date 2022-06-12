using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.DependencyInjection;

namespace ShoppingStore.Infrastructure.Api.Swagger
{
    public static class SwaggerConfigurationExtentions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo()
                    {
                        Title = "Library Api",
                        Version = "v1",
                        Description = "Through this Api you can access Database",
                        Contact = new OpenApiContact
                        {
                            Email = "mk7100000@gmail.com",
                            Name = "mohammad khaksari",
                            Url = new Uri("http://app.iran.liara.run"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri("http://app.iran.liara.run"),
                        },
                    });
                c.SwaggerDoc(
                    "v2",
                    new OpenApiInfo()
                    {
                        Title = "Library Api",
                        Version = "v2",
                        Description = "Through this Api you can access Database",
                        Contact = new OpenApiContact
                        {
                            Email = "mk7100000@gmail.com",
                            Name = "mohammad khaksari",
                            Url = new Uri("http://app.iran.liara.run"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri("http://app.iran.liara.run"),
                        },
                    });

                c.DescribeAllParametersInCamelCase();

                c.OperationFilter<RemoveVersionParameters>();
                c.DocumentFilter<SetVersionInPaths>();

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes<ApiVersionAttribute>(true)
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });

                c.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "Bearer");
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                });
            });
        }

        public static void UseSwaggerAndUI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Version 1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Api Version 2");
            });
        }
    }
}
