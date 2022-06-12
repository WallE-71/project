using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;

namespace ShoppingStore.Application.Services
{
    public class InsertRateSaller : IInsertRateSaller
    {
        private readonly IDataBaseContext _context;
        public InsertRateSaller(IDataBaseContext context)
        {
            _context = context;
        }

        public virtual async Task<bool> RateToSeller(int sellerId, string productId, int userId, int rate)
        {
            var currentRate = await _context.Ratings.Where(r => r.SellerId == sellerId && r.UserId == userId).ToListAsync();

            var order = await FindOrderByConditionAsync(o => o.UserId == userId && o.RemoveTime == null &&
                                             o.OrderDetails.Select(od => od.ProductId == productId).FirstOrDefault(),
                                             o => o.OrderBy(s => s.InsertTime));
            if (order.Count() != 0)
            {
                if (currentRate.Count() == 0)
                {
                    var rating = new Rating();
                    rating.UserId = userId;
                    rating.UserVote = rate;
                    rating.SellerId = sellerId;
                    rating.InsertTime = DateTime.Now;
                    await _context.Ratings.AddAsync(rating);
                }
                else
                {
                    currentRate.FirstOrDefault().UserVote = rate;
                    currentRate.FirstOrDefault().InsertTime = DateTime.Now;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Order>> FindOrderByConditionAsync(Expression<Func<Order, bool>> filter = null, Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = null)
        {
            IQueryable<Order> query = _context.Set<Order>();
            if (filter != null)
                query = query.Where(filter);
            if (orderBy != null)
                query = orderBy(query);
            return await query.ToListAsync();
        }
    }
}
