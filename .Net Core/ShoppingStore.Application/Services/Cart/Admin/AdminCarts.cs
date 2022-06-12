using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels.Cart;

namespace ShoppingStore.Application.Services
{
    public class AdminCarts : IAdminCarts
    {
        private readonly IDataBaseContext _context;
        public AdminCarts(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<CartViewModel>> GetPaginateCartsAsync(int offset, int limit, string orderBy, string searchText)
        {
            var carts = await _context.Carts.Include(c => c.CartItems).Include(c => c.User)
                                            .Where(c => (c.RemoveTime == null)
                                             || (c.User.FirstName.Contains(searchText)) || (c.User.LastName.Contains(searchText)))
                                            .OrderBy(orderBy).Skip(offset).Take(limit)
                                            .Select(c => new CartViewModel
                                            {
                                                Id = c.Id,
                                                ProductCount = c.CartItems.Count,
                                                SumAmount = c.CartItems.Sum(p => p.Price * p.Count),
                                                Customer = c.User.FirstName + " " + c.User.LastName
                                            }).AsNoTracking().ToListAsync();

            foreach (var item in carts)
                item.Row = ++offset;

            return carts;
        }

        public async Task<List<CartItemViewModel>> GetPaginateCartItemsAsync(int offset, int limit, string orderBy, string searchText, int cartId)
        {
            var cartItems = await _context.CartItems.Include(ci => ci.Product).ThenInclude(p => p.FileStores)
                                            .Where(p => (p.CartId == cartId) || (p.Product.Name == searchText))
                                            .OrderBy(orderBy).Skip(offset).Take(limit)
                                            .Select(ci => new CartItemViewModel
                                            {
                                                Id = ci.Id,
                                                Price = ci.Price,
                                                Count = ci.Count,
                                                ProductName = ci.Product.Name,
                                                Image = ci.Product.FileStores.FirstOrDefault(f => f.ProductId == ci.Product.Id).ImageName,
                                            }).AsNoTracking().ToListAsync();

            foreach (var item in cartItems)
                item.Row = ++offset;
            return cartItems;
        }
    }
}
