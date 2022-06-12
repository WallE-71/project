using System;
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
    public class AdminComments : IAdminComments
    {
        private readonly IDataBaseContext _context;
        public AdminComments(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<CommentViewModel>> GetPaginateCommentsAsync(int offset, int limit, string orderBy, string searchText, string productId, bool? isConfirm)
        {
            var convertConfirm = Convert.ToBoolean(isConfirm);
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();

            var comments = await _context.Comments.Include(c => c.Product)
                                   .Where(n => (isConfirm == null || (convertConfirm == true ? n.IsComplete : !n.IsComplete)) && n.ProductId.Contains(productId) && (n.Name.Contains(searchText)
                                       || n.Email.Contains(searchText) || (n.InsertTime >= getDateTimesForSearch.First() && n.InsertTime <= getDateTimesForSearch.Last())))
                                   .OrderBy(orderBy).Skip(offset).Take(limit)
                                   .Select(c => new CommentViewModel
                                   {
                                       Id = c.Id,
                                       Name = c.Name,
                                       Email = c.Email,
                                       IsComplete = c.IsComplete,
                                       Description = c.Description,
                                       ParentComment = c.Parent.Name,
                                       ProductName = c.Product.Name.Length >= 30 ? c.Product.Name.Substring(0, 30) + "..." : c.Product.Name,
                                       PersianInsertTime = c.InsertTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                                   }).AsNoTracking().ToListAsync();

            foreach (var item in comments)
                item.Row = ++offset;

            return comments;
        }

        public int CountUnAnsweredComments() => _context.Comments.Where(c => c.IsComplete == false).Count();
    }
}
