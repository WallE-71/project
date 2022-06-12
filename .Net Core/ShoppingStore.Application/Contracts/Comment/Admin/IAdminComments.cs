using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Comments;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminComments
    {
        Task<List<CommentViewModel>> GetPaginateCommentsAsync(int offset, int limit, string orderBy, string searchText, string productId, bool? isConfirm);
        int CountUnAnsweredComments();
    }
}
