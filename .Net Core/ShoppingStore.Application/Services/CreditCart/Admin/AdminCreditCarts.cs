using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.CreditCart;

namespace ShoppingStore.Application.Services
{
    public class AdminCreditCarts : IAdminCreditCarts
    {
        private readonly IDataBaseContext _context;
        public AdminCreditCarts(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<CreditCartViewModel>> GetPaginateCreditCartsAsync(int offset, int limit, string orderBy, string searchText)
        {
            //var getDateTimesForSearch = searchText.GetDateTimeForSearch();
            var creditCarts = await (from c in ((from c in _context.CreditCarts
                                                 //where (c.NationalId == searchText || c.BankCode == searchText)
                                                 select (new
                                                 {
                                                     c.Credit,
                                                     c.BankCode,
                                                     c.NationalId,
                                                     c.InsertTime,
                                                     UserName = _context.Users.Where(u => u.Id == c.UserId).FirstOrDefault().UserName,
                                                     SellerName = _context.Sellers.Where(u => u.Id == c.SellerId).FirstOrDefault().Name,
                                                 })).Skip(offset).Take(limit))
                                     select (new CreditCartViewModel
                                     {
                                         Credit = c.Credit,
                                         BankCode = c.BankCode,
                                         NationalId = c.NationalId,
                                         stringCredit = c.Credit.ToString().En2Fa(),
                                         Owner = c.SellerName != null ? c.SellerName : c.UserName,
                                         PersianInsertTime = c.InsertTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                                     })).OrderBy(orderBy).AsNoTracking().ToListAsync();

            foreach (var item in creditCarts)
                item.Row = ++offset;

            return creditCarts;
        }
    }
}