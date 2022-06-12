using System.Threading.Tasks;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminUsers
    {
        Task<string> GetUserImageAsync(int? userId);
        Task<int[]> GetRoleIdsAsync(int? userId);
    }
}
