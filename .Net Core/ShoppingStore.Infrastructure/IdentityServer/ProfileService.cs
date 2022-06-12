using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Infrastructure.IdentityServer
{
    public class ProfileService : IProfileService
    {
        public const string UserName = "userName";
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;
        public ProfileService(
            UserManager<User> userManager,
            IUserClaimsPrincipalFactory<User> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            if (user.UserName != null)
                claims.Add(new Claim(UserName, user.UserName));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);

            context.IsActive = (user != null) && user.IsActive;
        }
    }
}
