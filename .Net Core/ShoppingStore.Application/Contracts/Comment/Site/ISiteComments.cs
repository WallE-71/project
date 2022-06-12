using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Comments;

namespace ShoppingStore.Application.Contracts
{
    public interface ISiteComments
    {
        Task<List<CommentViewModel>> GetProductCommentsAsync(string productId);
        CommentViewModel NumberOfLikeAndDislike(int commentId);
    }
}
