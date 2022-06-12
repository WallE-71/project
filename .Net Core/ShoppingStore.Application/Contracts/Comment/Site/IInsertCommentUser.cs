using System.Threading.Tasks;
using ShoppingStore.Application.ViewModels.Comments;

namespace ShoppingStore.Application.Contracts
{
    public interface IInsertCommentUser
    {
        Task SendComment(string name, string email, string description, string productId, int? parentCommentId);
    }
}
