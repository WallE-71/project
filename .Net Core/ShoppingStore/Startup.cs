using System.IO;
using System.Linq;
using Coravel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using ShoppingStore.Areas.Admin;
using ShoppingStore.Infrastructure;
using ShoppingStore.Infrastructure.Extentions;
using ShoppingStore.Infrastructure.Api.Swagger;
using ShoppingStore.Infrastructure.IdentityServer;
using ShoppingStore.Infrastructure.Api.Middlewares;
using ShoppingStore.Application.ViewModels.Settings;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly SiteSettings SiteSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SiteSettings = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(Configuration);
            services.AddCustomIdentityServices();
            services.ConfigureWritable<SiteSettings>(Configuration.GetSection("SiteSettings"));
            services.AddCustomAuthentication(SiteSettings);
            services.AddCustomeIdentityServer();
            services.AddApiVersioning();
            services.AddAutoMapper();
            services.AddScheduler();
            services.AddSwagger();
            services.AddSignalR();
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ConstantPolicies.DynamicPermission, policy =>
                            policy.Requirements.Add(new DynamicPermissionRequirement()));
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Error";
                options.AccessDeniedPath = "/Admin/Manage/AccessDenied";
            });

            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/../Areas/Admin/Manage/SignIn", "");
            });

            //services.AddSpaStaticFiles(spa =>
            //{
            //    spa.RootPath = "ClientApp/dist";
            //});
            //services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");

            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(builder =>
            //    {
            //        var frontendURL = Configuration.GetValue<string>("Frontend_Url");
            //        builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader()
            //               .WithExposedHeaders(new string[] { "totalAmountOfRecords" });
            //    });
            //});

            //services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            //services.AddOpenApiDocument(configure =>
            //{
            //    configure.Title = "CleanArchitecture API";
            //    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            //    {
            //        Type = OpenApiSecuritySchemeType.ApiKey,
            //        Name = "Authorization",
            //        In = OpenApiSecurityApiKeyLocation.Header,
            //        Description = "Type into the textbox: Bearer {your JWT token}."
            //    });
            //    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            //});

            bool requireHttpsMetadata = false;
            bool.TryParse(Configuration["IsProduction"], out requireHttpsMetadata);
            if (requireHttpsMetadata && !string.IsNullOrWhiteSpace(Configuration["HttpsRedirectionPort"]))
            {
                services.AddHttpsRedirection(options =>
                {
                    options.HttpsPort = int.Parse(Configuration["HttpsRedirectionPort"]);
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/docs"), appBuilder =>
            {
                appBuilder.UseCustomExceptionHandler();
            });
            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api") || !context.Request.Path.StartsWithSegments("/docs"), appBuilder =>
            {
                if (env.IsDevelopment())
                    app.UseDeveloperExceptionPage();
                else
                    app.UseExceptionHandler("/Error404");
            });

            var cachePeriod = env.IsDevelopment() ? "600" : "605800";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CacheFiles")),
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={cachePeriod}");
                },
                RequestPath = "/CacheFiles",
            });

            app.UseCustomIdentityServices();
            app.UseHealthChecks("/health");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
                app.UseSpaStaticFiles();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseRouting();
            app.UseSwaggerAndUI();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Error404";
                    await next();
                }
            });

            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {
                scheduler.Schedule<SendWeeklyNewsLetter>().EveryFifteenMinutes()/*.Cron("29 20 * * 5")*/;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Manage}/{action=SignIn}"
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}"
                );
                endpoints.MapRazorPages();
                endpoints.MapHub<MessageHub>("/messagehub");
                endpoints.MapHub<OnlineCountHub>("/onlinecount");
            });

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";
            //    if (env.IsDevelopment())
            //    {
            //        //spa.UseAngularCliServer(npmScript: "start");
            //        spa.Options.StartupTimeout = TimeSpan.FromSeconds(120);
            //        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
            //    }
            //});
        }
    }
}
