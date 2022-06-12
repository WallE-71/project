using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Order;

namespace ShoppingStore.Application.Services
{
    public class AdminOrders : IAdminOrders
    {
        private readonly IDataBaseContext _context;
        public AdminOrders(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<OrderViewModel>> GetPaginateOrdersAsync(int offset, int limit, string orderBy, string searchText)
        {
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();
            var orders = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                                .Where(o => o.RequestPayId.Contains(searchText) || o.User.FirstName.Contains(searchText) || o.User.LastName.Contains(searchText)
                                    || (o.States == searchText.GetOrderStateForSearch())
                                    || (o.InsertTime >= getDateTimesForSearch.First() && o.InsertTime <= getDateTimesForSearch.Last()))
                                .Select(o => new OrderViewModel
                                {
                                    Id = o.Id,
                                    Quantity = o.Quantity,
                                    OrderState = o.States,
                                    AmountPaid = o.AmountPaid,
                                    InsertTime = o.InsertTime,
                                    RequestPayId = o.RequestPayId,
                                    StringQuantity = o.Quantity.ToString().En2Fa(),
                                    StringAmountPaid = o.AmountPaid.ToString().En2Fa(),
                                    CustomerName = o.User.FirstName + " " + o.User.LastName,
                                    ProductName = o.OrderDetails.Select(od => od.Product.Name).FirstOrDefault(),
                                    PersianInsertTime = o.InsertTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                                    TotalCart = _context.Orders.Where(c => c.RemoveTime == null && o.Id == _context.Orders.FirstOrDefault(c => c.Id == o.Id).Id).Count().ToString().En2Fa(),
                                    Status = o.States == OrderState.Processing ? "در حال پردازش" : o.States == OrderState.Confirmed ? "تایید شده" : o.States == OrderState.Transmission ? "در حال ارسال" : "آماده تحویل",
                                    OrderDetails = o.OrderDetails.Select(o => new OrderDetailViewModel
                                    {
                                        Count = o.Count,
                                        Price = o.Price,
                                        OrderDetailId = o.Id,
                                        ProductId = o.ProductId,
                                        ProductName = o.Product.Name,
                                    }).ToList(),
                                }).OrderBy(orderBy).Skip(offset).Take(limit).AsQueryable().AsNoTracking().ToListAsync();

            foreach (var item in orders)
                item.Row = ++offset;

            return orders;
        }

        public long TodaySaleAmount()
        {
            var date = DateTime.Now.Date;
            var endToday = date.EndOfDay();
            var amountAllOrders = (from o in _context.Orders
                                         where (o.RemoveTime == null && o.AmountPaid != 0 && /*o.IsComplete &&*/
                                               (o.InsertTime <= endToday && o.InsertTime >= date))
                                         select (new
                                         {
                                             AmountPaid = o.AmountPaid,
                                         })).ToList();

            return (long)amountAllOrders.Select(n => n.AmountPaid).AsEnumerable().Sum();
        }
    }

    public static class OrderExtension
    {
        public static OrderState GetOrderStateForSearch(this string searchText)
        {
            var processing = OrderState.Processing;
            var confirmed = OrderState.Confirmed;
            var transmission = OrderState.Transmission;
            var delivered = OrderState.Delivered;

            if (searchText.Contains("در حال پردازش"))
                return (OrderState)1;
            if (searchText.Contains("تایید شده"))
                return (OrderState)2;
            if (searchText.Contains("در حال ارسال"))
                return (OrderState)3;
            if (searchText.Contains("آماده تحویل"))
                return (OrderState)4;
            return processing;
        }
    }
}
