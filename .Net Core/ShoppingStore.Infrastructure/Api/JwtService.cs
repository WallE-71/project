using System;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShoppingStore.Infrastructure.Contract;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Application.ViewModels.Settings;
using ShoppingStore.Application.ViewModels.Api.SignIn;
using ShoppingStore.Infrastructure.Contracts;

namespace ShoppingStore.Infrastructure.Api
{
    public class JwtService : IJwtService
    {
        public readonly SiteSettings _siteSettings;
        public readonly IApplicationUserManager _userManager;
        public readonly IApplicationRoleManager _roleManager;
        public JwtService(
            IApplicationUserManager userManager,
            IApplicationRoleManager roleManager,
            IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _siteSettings = siteSettings.Value;
        }

        public AuthenticationDto GenerateTokenAsync(User user)
        {
            var expiration = DateTime.UtcNow.AddYears(1);
            var secretKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _siteSettings.JwtSettings.Issuer,
                audience: _siteSettings.JwtSettings.Audience,
                notBefore: DateTime.Now.AddMinutes(_siteSettings.JwtSettings.NotBeforeMinutes),
                claims: GetClaimsAsync(user),
                expires: expiration,
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return new AuthenticationDto
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = expiration
            };
        }

        public IEnumerable<Claim> GetClaimsAsync(User user)
        {
            var Claims = new List<Claim>()
            {
                new Claim("email",user.Email),
                new Claim("address",user.Address),
                new Claim("id",user.Id.ToString()),
                new Claim("lastName",user.LastName),
                new Claim("firstName",user.FirstName),
                new Claim("phoneNumber",user.PhoneNumber),
                new Claim(new ClaimsIdentityOptions().SecurityStampClaimType,user.SecurityStamp),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            return Claims;
        }
    }
}
