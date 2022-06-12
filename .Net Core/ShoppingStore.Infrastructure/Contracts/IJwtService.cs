using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Application.ViewModels.Api.SignIn;

namespace ShoppingStore.Infrastructure.Contract
{
    public interface IJwtService
    {
        AuthenticationDto GenerateTokenAsync(User User);
    }
}
