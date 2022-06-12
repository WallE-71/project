using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Order;

namespace ShoppingStore.Application.Services
{
    public class SiteOrders : ISiteOrders
    {
        private readonly IDataBaseContext _context;
        public SiteOrders(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<ResultViewModel<List<UserOrderViewModel>>> GetUserOrdersAsync(int userId, int offset, int limit)
        {
            var user = _context.Users.Find(userId);
            var orders = await _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).ThenInclude(p => p.FileStores)
                            .Where(o => o.UserId == userId && o.RemoveTime == null)
                            .OrderByDescending(o => o.Id).Skip(offset).Take(limit)
                            .Select(o => new UserOrderViewModel
                            {
                                Id = o.Id,
                                States = o.States,
                                AmountPaid = o.AmountPaid,
                                RequestPayId = o.RequestPayId,
                                PhoneNumber = user.PhoneNumber,
                                Customer = user.FirstName + " " + user.LastName,
                                PersianInsertTime = o.InsertTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                                Deadline = DateTimeExtensions.DateTimeWithOutMilliSecends(Convert.ToDateTime(o.InsertTime).AddDays(7).Date),
                                OrderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                                {
                                    Count = od.Count,
                                    Price = od.Price,
                                    OrderDetailId = od.Id,
                                    ProductId = od.ProductId,
                                    ProductName = od.Product.Name,
                                    ImageName = od.Product.FileStores.FirstOrDefault(f => f.ProductId == od.ProductId).ImageName,
                                }).ToList(),
                            }).ToListAsync();

            foreach (var order in orders)
                order.Row = ++offset;

            return new ResultViewModel<List<UserOrderViewModel>>()
            {
                Data = orders,
                IsSuccess = true,
            };
        }

        public async Task<ResultViewModel> RequestAddNewOrderAsync(RequestAddNewOrder request)
        {
            var product = new Domain.Entities.Product();
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id== request.UserId);
            var requestPay = await _context.RequestPays.FirstOrDefaultAsync(r=>r.Id== request.RequestPayId);
            var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(c => c.Product)
                                     .Where(c => c.Id == request.CartId).FirstOrDefaultAsync();

            cart.IsComplete = true;
            requestPay.IsComplete = true;
            requestPay.RefId = request.RefId;
            requestPay.InsertTime = DateTime.Now;
            requestPay.Authority = request.Authority;

            var maxOrder = new Domain.Entities.Order();
            var orders = await _context.Orders.AsNoTracking().ToListAsync();
            if (orders.Count != 0)
                maxOrder.Id = orders.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
            else
                maxOrder.Id = 1;

            var order = new Domain.Entities.Order()
            {
                User = user,
                UserId = user.Id,
                Id = maxOrder.Id,
                RequestPay = requestPay,
                InsertTime = DateTime.Now,
                AmountPaid = requestPay.Amount,
                States = OrderState.Processing,
                RequestPayId = request.RequestPayId,
                Quantity = cart.CartItems.Where(ci => ci.RemoveTime == null).Count()
            };
            await _context.Orders.AddAsync(order);

            var maxOrderDetail = new OrderDetail();
            var allOrderDetails = await _context.OrderDetails.AsNoTracking().ToListAsync();
            if (allOrderDetails.Count != 0)
                maxOrderDetail.Id = allOrderDetails.OrderByDescending(c => c.Id).First().Id + 1;
            else
                maxOrderDetail.Id = 1;
            var id = maxOrderDetail.Id;
            var orderDetails = new List<OrderDetail>();
            foreach (var item in cart.CartItems)
            {
                var orderDetail = new OrderDetail()
                {
                    Id = id,
                    Order = order,
                    Count = item.Count,
                    OrderId = order.Id,
                    Product = item.Product,
                    Price = item.Product.Price,
                    ProductId = item.Product.Id,
                };
                id++;
                orderDetails.Add(orderDetail);
                product = await _context.Products.FindAsync(item.Product.Id);
                product.Stock -= item.Count;
                product.NumberOfSale += item.Count;
                _context.Products.Update(product);
            }
            _context.OrderDetails.AddRange(orderDetails);

            var invoice = new Invoice();
            var invoices = await _context.Invoices.ToListAsync();
            if (invoices.Count() != 0)
                invoice.Id = invoices.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
            else
                invoice.Id = 1;
            invoice.InsertTime = DateTime.Now;
            invoice.Name = order.Id.ToString();
            invoice.Description = request.RequestPayId;
            await _context.SaveChangesAsync();

            return new ResultViewModel()
            {
                IsSuccess = true,
            };
        }

        public ResultViewModel RemoveOrder(int orderId, int userId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.UserId == userId && o.Id == orderId && o.RemoveTime == null);
            if (order != null)
            {
                order.RemoveTime = DateTime.Now;
                _context.SaveChanges();
                return new ResultViewModel()
                {
                    IsSuccess = true,
                };
            }
            return new ResultViewModel()
            {
                IsSuccess = false,
            };
        }
    }
}
