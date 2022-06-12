using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Infrastructure.IdentityServer
{
    public static class IdentityServerConfig
    {
        public const string Roles = "roles";
        public const string Permission = "permission";
        public const string ApiName = "snapmarket_api";
        public const string ClientID = "snapmarket_spa";
        public const string SwaggerClientID = "swaggerui";
        public const string ApiFriendlyName = "SnapMarke API";

        public static IServiceCollection AddCustomeIdentityServer(this IServiceCollection services)
        {
            //services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
            //                            .AddEntityFrameworkStores<SnapMarketDBContext>();

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.UserInteraction.LoginUrl = "/login";
                options.UserInteraction.LogoutUrl = "/logout";
            }).AddAspNetIdentity<User>()
                .AddInMemoryPersistedGrants()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(GetClients())
                .AddProfileService<ProfileService>()
                .AddInMemoryApiResources(GetApiResources())
                .AddInMemoryIdentityResources(GetIdentityResources());

            //services.AddAuthentication().AddIdentityServerJwt();
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()
            //    .AddApiAuthorization<User, SnapMarketDBContext>((config) =>
            //    {
            //        config.Clients[0].AccessTokenLifetime = 3600;
            //    });

            return services;
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource(Roles, new List<string> { JwtClaimTypes.Role })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(ApiName)
                {
                    UserClaims =
                    {
                        Permission,
                        JwtClaimTypes.Role,
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email,
                        JwtClaimTypes.PhoneNumber,
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = ClientID,
                    RequireClientSecret = false,
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    AllowedScopes =
                    {
                        Roles,
                        ApiName,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 1800,
                    SlidingRefreshTokenLifetime = 86400,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding
                }
            };
        }
    }
}
