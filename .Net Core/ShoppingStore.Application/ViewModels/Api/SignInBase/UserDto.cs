using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.Api.SignIn
{
    public class UserDto
    {
        public string browserId { get; set; }
        public AuthenticationDto token { get; set; }
    }
}
