using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.MessageUser;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminMessageUsers
    {
        int CountUnAnsweredComments();
        Task<List<MessageUsersViewModel>> GetPaginateMessagesAsync(int offset, int limit, string orderBy, string searchText, int userId, bool? isRegisterCode);
    }
}
