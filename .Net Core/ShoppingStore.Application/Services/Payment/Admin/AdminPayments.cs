using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using static ShoppingStore.Application.ViewModels.RequestPayViewModel;

namespace ShoppingStore.Application.Services
{
    public class AdminPayments : IAdminPayments
    {
        private readonly IDataBaseContext _context;
        public AdminPayments(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<ItemRequestPay>> GetPaginateRequestPaysAsync(int offset, int limit, bool? sortAsc, string searchText)
        {
            List<ItemRequestPay> requestPays;
            //var getDateTimesForSearch = searchText.GetDateTimeForSearch();
            requestPays = await _context.RequestPays
                                //.Where(r => r.InsertTime == null ? r.InsertTime >= getDateTimesForSearch.First() && r.InsertTime <= getDateTimesForSearch.Last() : false)
                                .Select(r => new ItemRequestPay
                                {
                                    Id = r.Id,
                                    IsComplete = r.IsComplete,
                                    DisplayAmount = r.Amount.ToString().En2Fa(),
                                    PersianUpdateTime = r.UpdateTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                                }).Skip(offset).Take(limit).AsNoTracking().ToListAsync();

            if (sortAsc != null)
                requestPays = requestPays.OrderBy(t => (sortAsc == true && sortAsc != null) ? t.Amount : 0).OrderByDescending(t => (sortAsc == false && sortAsc != null) ? t.Amount : 0).ToList();

            foreach (var item in requestPays)
                item.Row = ++offset;
            return requestPays;
        }
    }
}
