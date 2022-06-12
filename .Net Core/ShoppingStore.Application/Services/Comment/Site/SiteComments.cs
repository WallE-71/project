using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Comments;

namespace ShoppingStore.Application.Services
{
    public class SiteComments: ISiteComments
    {
        private readonly IDataBaseContext _context;
        public SiteComments(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<CommentViewModel>> GetProductCommentsAsync(string productId)
        {
            var comments = await (from c in _context.Comments.Include(c => c.Likes)
                                  where ((c.ParentId == null || c.ParentId == 0) && c.ProductId == productId && c.IsComplete == true)
                                  select new CommentViewModel
                                  {
                                      Id = c.Id,
                                      Name = c.Name,
                                      Email = c.Email,
                                      ProductId = c.ProductId,
                                      Description = c.Description,
                                      ParentCommentId = c.ParentId,
                                      PersianInsertTime = c.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت HH:mm:ss"),
                                  }).ToListAsync();

            foreach (var item in comments)
            {
                item.NumberOfLike = NumberOfLikeAndDislike(item.Id).NumberOfLike;
                item.NumberOfDisLike = NumberOfLikeAndDislike(item.Id).NumberOfDisLike;
                await BindSubComments(item);
            }
            return comments;
        }

        public async Task BindSubComments(CommentViewModel viewModel)
        {
            var subComments = await (from c in _context.Comments.Include(p => p.Likes)
                                     where (c.ParentId == viewModel.Id && c.IsComplete == true)
                                     select new CommentViewModel
                                     {
                                         Id = c.Id,
                                         Name = c.Name,
                                         Email = c.Email,
                                         ProductId = c.ProductId,
                                         Description = c.Description,
                                         ParentCommentId = c.ParentId,
                                         PersianInsertTime = c.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت HH:mm:ss"),
                                     }).ToListAsync();

            if (viewModel.SubComments != null)
            {
                foreach (var item in subComments)
                {
                    item.NumberOfLike = NumberOfLikeAndDislike(item.Id).NumberOfLike;
                    item.NumberOfDisLike = NumberOfLikeAndDislike(item.Id).NumberOfDisLike;
                    viewModel.SubComments.Add(item);
                }
            }
            else
            {
                viewModel.SubComments = new List<CommentViewModel>();
                foreach (var item in subComments)
                {
                    item.NumberOfLike = NumberOfLikeAndDislike(item.Id).NumberOfLike;
                    item.NumberOfDisLike = NumberOfLikeAndDislike(item.Id).NumberOfDisLike;
                    await BindSubComments(item);
                    viewModel.SubComments.Add(item);
                }
            }
        }

        public CommentViewModel NumberOfLikeAndDislike(int commentId)
        {
            return (from c in _context.Comments.Include(p => p.Likes)
                    where (c.Id == commentId)
                    select new CommentViewModel
                    {
                        NumberOfLike = c.Likes.Where(l => l.IsLiked == true).Count(),
                        NumberOfDisLike = c.Likes.Where(l => l.IsLiked == false).Count()
                    }).FirstOrDefault();
        }
    }
}
