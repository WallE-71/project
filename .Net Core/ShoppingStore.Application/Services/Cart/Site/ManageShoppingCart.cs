using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Cart;

namespace ShoppingStore.Application.Services
{
    public class ManageShoppingCart: IManageShoppingCart
    {
        private readonly ISiteCarts _siteCarts;
        private readonly IDataBaseContext _context;
        public ManageShoppingCart(IDataBaseContext context)
        {
            _context = context;
            _siteCarts = new SiteCarts(_context);
        }

        public async Task<CartViewModel> ManageCart(CartViewModel result, string browserId)
        {
            if (result.Id == 0)
            {
                var id = 0;
                var carts = await _context.Carts.AsNoTracking().ToListAsync();
                if (carts.Count() != 0)
                    id = carts.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                var cart = new Cart
                {
                    IsComplete = false,
                    BrowserId = browserId,
                    UserId = result.UserId,
                    Id = carts.Count() == 0 ? 1 : id,
                    CartItems = new List<CartItem>(),
                    InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
                return result;
            }
            else
            {
                var viewModel = new CartViewModel
                {
                    SumAmount = result.SumAmount,
                    CartItems = result.CartItems,
                    ProductCount = result.ProductCount,
                };
                return viewModel;
            }
        }
    }
}
