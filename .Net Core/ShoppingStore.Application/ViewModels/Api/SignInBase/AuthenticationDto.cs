using System;

namespace ShoppingStore.Application.ViewModels.Api.SignIn
{
    public class AuthenticationDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
