using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingStore.Persistence;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Application.ViewModels.Settings;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Infrastructure.Identity
{
    public class IdentityDbInitializer : IIdentityDbInitializer
    {
        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        private readonly IApplicationUserManager _applicationUserManager;
        private readonly ILogger<IdentityDbInitializer> _logger;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IServiceScopeFactory _scopeFactory;
        public IdentityDbInitializer(
            IApplicationUserManager applicationUserManager,
            IServiceScopeFactory scopeFactory,
            IApplicationRoleManager roleManager,
            IOptionsSnapshot<SiteSettings> adminUserSeedOptions,
            ILogger<IdentityDbInitializer> logger)
        {
            _applicationUserManager = applicationUserManager;
            _scopeFactory = scopeFactory;
            _roleManager = roleManager;
            _adminUserSeedOptions = adminUserSeedOptions;
            _logger = logger;
        }

        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
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
                var identityDbSeedData = serviceScope.ServiceProvider.GetService<IIdentityDbInitializer>();
                var result = identityDbSeedData.SeedDatabaseAsync().Result;

                if (result == IdentityResult.Failed())
                    throw new InvalidOperationException(result.DumpErrors());
            }
        }

        public async Task<IdentityResult> SeedDatabaseAsync()
        {
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;
            var email = adminUserSeed.Email;
            var name = adminUserSeed.Username;
            var credit = adminUserSeed.Credit;
            var address = adminUserSeed.Address;
            var lastName = adminUserSeed.LastName;
            var password = adminUserSeed.Password;
            var roleName = adminUserSeed.RoleName;
            var firstName = adminUserSeed.FirstName;
            var phoneNumber = adminUserSeed.PhoneNumber;
            var thisMethodName = nameof(SeedDatabaseAsync);

            var adminUser = await _applicationUserManager.FindByNameAsync(name);
            if (adminUser != null)
            {
                _logger.LogInformation($"{thisMethodName}: adminUser already exists.");
                return IdentityResult.Success;
            }

            var adminRole = await _roleManager.FindByNameAsync(roleName);
            if (adminRole == null)
            {
                adminRole = new Role(roleName);
                var adminRoleResult = await _roleManager.CreateAsync(adminRole);

                if (adminRoleResult == IdentityResult.Failed())
                {
                    _logger.LogError($"{thisMethodName}: adminRole CreateAsync failed. {adminRoleResult.DumpErrors()}");
                    return IdentityResult.Failed();
                }
            }
            else
                _logger.LogInformation($"{thisMethodName}: adminRole already exists.");

            adminUser = new User
            {
                Email = email,
                UserName = name,
                Address= address,
                LastName = lastName,
                FirstName = firstName,
                Gender = GenderType.Male,
                PhoneNumber = phoneNumber,
                InsertTime = DateTime.Now,
                IsActive = true,
                EmailConfirmed = true,
                LockoutEnabled = true,
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = true,
            };

            var adminUserResult = await _applicationUserManager.CreateAsync(adminUser, password);
            if (adminUserResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser CreateAsync failed. {adminUserResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            var setLockoutResult = await _applicationUserManager.SetLockoutEnabledAsync(adminUser, enabled: false);
            if (setLockoutResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser SetLockoutEnabledAsync failed.");
                return IdentityResult.Failed();
            }

            var addToRoleResult = await _applicationUserManager.AddToRoleAsync(adminUser, adminRole.Name);
            if (addToRoleResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser AddToRoleAsync failed. {addToRoleResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            var addToClaimsResult = await _applicationUserManager.AddClaimsAsync(adminUser, new List<Claim>
                                  { new Claim(ConstantPolicies.DynamicPermissionClaimType, "Admin:DynamicAccess:Index"), 
                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, "Admin:UserManager:Index"),

                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":DynamicAccess:Get"),
                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":DynamicAccess:Add"),
                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":DynamicAccess:Delete"),

                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":UserApi:ShowAllUsers"),
                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":UserApi:Get"), new Claim(ConstantPolicies.DynamicPermissionClaimType, ":UserApi:Create"),
                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":UserApi:Update"), new Claim(ConstantPolicies.DynamicPermissionClaimType, ":UserApi:Delete"),

                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":CategoryApi:ShowAllCategories"), new Claim(ConstantPolicies.DynamicPermissionClaimType, ":CategoryApi:Get"),
                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":CategoryApi:CreateOrUpdate"), new Claim(ConstantPolicies.DynamicPermissionClaimType, ":CategoryApi:Delete"),

                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":ProductApi:ShowAllProducts"), new Claim(ConstantPolicies.DynamicPermissionClaimType, ":ProductApi:Get"),
                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":ProductApi:CreateOrUpdate"), new Claim(ConstantPolicies.DynamicPermissionClaimType, ":ProductApi:Delete"),

                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":TagApi:ShowAllTags"), new Claim(ConstantPolicies.DynamicPermissionClaimType, ":TagApi:Get"),
                                    new Claim(ConstantPolicies.DynamicPermissionClaimType, ":TagApi:CreateOrUpdate"), new Claim(ConstantPolicies.DynamicPermissionClaimType, ":TagApi:Delete")
                                  });

            if (addToClaimsResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser AddToClaimsAsync failed. {addToClaimsResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }
    }
}
