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
using ShoppingStore.Application.ViewModels.Cart;

namespace ShoppingStore.Application.Services
{
    public class SiteCarts : ISiteCarts
    {
        private readonly IDataBaseContext _context;
        public SiteCarts(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<CartViewModel> GetCartAsync(int userId, string browserId)
        {
            var cart = await (from c in _context.Carts.Include(c => c.User).Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                              where (c.User.Id == userId && c.BrowserId == browserId && c.IsComplete == false && c.RemoveTime == null)
                              orderby (c.InsertTime)
                              select (new CartViewModel
                              {
                                  Id = c.Id,
                                  Customer = c.User.FirstName + c.User.LastName,
                                  ProductCount = c.CartItems.Where(ci => ci.RemoveTime == null).Count(),
                                  SumAmount = c.CartItems.Where(ci => ci.RemoveTime == null).Sum(ci => ci.Price * ci.Count)
                              })).AsNoTracking().FirstOrDefaultAsync();

            if (cart != null)
                cart.CartItems = await BindCartItems(cart.Id);
            else
                cart = new CartViewModel();
            return cart;
        }

        public async Task<List<CartItemViewModel>> BindCartItems(int id)
        {
            var nameOfColor = "";
            var viewModels = new List<CartItemViewModel>();
            var cartItems = await (from ci in _context.CartItems
                                   where (ci.CartId == id && ci.RemoveTime == null)
                                   join pc in _context.ProductColors on ci.ProductId equals pc.ProductId into aa
                                   from productColor in aa.DefaultIfEmpty()
                                   join co in _context.Colors on productColor.ColorId equals co.Id into bb
                                   from color in bb.DefaultIfEmpty()
                                   select (new CartItemViewModel
                                   {
                                       Id = ci.Id,
                                       Count = ci.Count,
                                       Price = ci.Price,
                                       ProductId = ci.ProductId,
                                       ProductName = ci.Product.Name,
                                       SelectedColor = ci.ProductColor,
                                       NameOfColor = color != null ? color.Name : "",
                                   })).ToListAsync();

            var cartItemGroup = cartItems.GroupBy(g => g.Id).Select(g => new { Id = g.Key, CartItemGroup = g });
            foreach (var item in cartItemGroup)
            {
                nameOfColor = "";
                foreach (var a in item.CartItemGroup.Select(a => a.NameOfColor).Distinct())
                {
                    if (nameOfColor == "")
                        nameOfColor = a;
                    else
                        nameOfColor = nameOfColor + " , " + a;
                }

                var viewModel = new CartItemViewModel()
                {
                    Id = item.Id,
                    NameOfColor = nameOfColor,
                    Count = item.CartItemGroup.First().Count,
                    Price = item.CartItemGroup.First().Price,
                    Colors = item.CartItemGroup.First().Colors,
                    ProductId = item.CartItemGroup.First().ProductId,
                    ProductName = item.CartItemGroup.First().ProductName,
                    SelectedColor = item.CartItemGroup.First().SelectedColor,
                };
                viewModels.Add(viewModel);
            }
            return viewModels;
        }

        public async Task<ResultViewModel<CartItem>> AddToCartAsync(string productId, string browserId, string color)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                var cart = await _context.Carts.Where(c => c.BrowserId == browserId && c.IsComplete == false && c.RemoveTime == null).FirstOrDefaultAsync();
                var cartItem = await _context.CartItems.Where(ci => ci.ProductId == productId && ci.CartId == cart.Id && ci.RemoveTime == null).FirstOrDefaultAsync();
                if (cartItem != null)
                {
                    if (color.HasValue())
                        cartItem.ProductColor = "#" + color;
                    else
                    {
                        if (cartItem.RemoveTime != null)
                        {
                            cartItem.Count = 0;
                            cartItem.RemoveTime = null;
                        }
                        cartItem.Count += 1;
                    }
                    cartItem.UpdateTime = DateTime.Now;
                    _context.CartItems.Update(cartItem);
                }
                else
                {
                    var id = 0;
                    var cartItems = await _context.CartItems.AsNoTracking().ToListAsync();
                    if (cartItems.Count != 0)
                        id = cartItems.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                    var newCartItem = new CartItem()
                    {
                        Count = 1,
                        Cart = cart,
                        CartId = cart.Id,
                        Product = product,
                        Price = product.Price,
                        ProductId = product.Id,
                        Id = cartItems.Count != 0 ? id : 1,
                    };
                    cartItem = newCartItem;
                    await _context.CartItems.AddAsync(newCartItem);
                }
                await _context.SaveChangesAsync();

                return new ResultViewModel<CartItem>()
                {
                    Data = cartItem,
                    IsSuccess = true,
                    Message = $"محصول  {product.Name} با موفقیت به سبد خرید شما اضافه شد ",
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<CartItem>()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResultViewModel> Increase(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            cartItem.Count++;
            await _context.SaveChangesAsync();
            return new ResultViewModel()
            {
                IsSuccess = true,
            };
        }

        public async Task<ResultViewModel> Decrease(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem.Count <= 1)
            {
                return new ResultViewModel()
                {
                    IsSuccess = false,
                };
            }
            cartItem.Count--;
            await _context.SaveChangesAsync();

            return new ResultViewModel()
            {
                IsSuccess = true,
            };
        }

        public async Task<ResultViewModel> RemoveFromCart(string productId, string browserId)
        {
            var cartitem = await _context.CartItems.Where(c => c.ProductId == productId && c.Cart.BrowserId.ToString() == browserId && c.Cart.IsComplete == false).FirstOrDefaultAsync();
            if (cartitem != null)
            {
                cartitem.RemoveTime = DateTime.Now;
                await _context.SaveChangesAsync();

                return new ResultViewModel
                {
                    IsSuccess = true,
                    Message = "محصول از سبد خرید شما حذف شد"
                };
            }
            else
            {
                return new ResultViewModel
                {
                    IsSuccess = false,
                    Message = "محصول یافت نشد"
                };
            }
        }

        public async Task<ResultViewModel> RemoveAllFromCart(string browserId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).Where(c => c.BrowserId.ToString() == browserId && c.IsComplete == false).FirstOrDefaultAsync();
            if (cart != null)
            {
                var cartItems = cart.CartItems.Where(ci => ci.RemoveTime == null).ToList();
                foreach (var item in cartItems)
                    item.RemoveTime = DateTime.Now;

                cart.RemoveTime = DateTime.Now;
                await _context.SaveChangesAsync();

                return new ResultViewModel
                {
                    IsSuccess = true
                };
            }
            else
            {
                return new ResultViewModel
                {
                    IsSuccess = false
                };
            }
        }
    }
}
