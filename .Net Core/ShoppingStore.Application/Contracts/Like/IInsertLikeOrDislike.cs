using System.Threading.Tasks;

namespace ShoppingStore.Application.Contracts
{
    public interface IInsertLikeOrDislike
    {
        Task LikeOrDisLike(int commentId, string browserId, bool isLiked);
    }
}
