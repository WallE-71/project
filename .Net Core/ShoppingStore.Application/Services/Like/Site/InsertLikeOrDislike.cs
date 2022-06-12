using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;

namespace ShoppingStore.Application.Services
{
    public class InsertLikeOrDislike : IInsertLikeOrDislike
    {
        private readonly IDataBaseContext _context;
        public InsertLikeOrDislike(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task LikeOrDisLike(int commentId, string browserId, bool isLiked)
        {
            var likeOrDislike = await _context.Likes.FirstOrDefaultAsync(l => l.CommentId == commentId && l.BrowserId == browserId);
            if (likeOrDislike == null)
            {
                likeOrDislike = new Like { CommentId = commentId, BrowserId = browserId, IsLiked = isLiked };
                await _context.Likes.AddAsync(likeOrDislike);
            }
            else
                likeOrDislike.IsLiked = isLiked;
            await _context.SaveChangesAsync();
        }
    }
}
