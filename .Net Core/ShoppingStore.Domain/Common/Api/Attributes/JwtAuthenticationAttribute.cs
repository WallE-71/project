using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ShoppingStore.Domain.Common.Api.Attributes
{
    public class JwtAuthenticationAttribute : AuthorizeAttribute
    {
        public JwtAuthenticationAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
