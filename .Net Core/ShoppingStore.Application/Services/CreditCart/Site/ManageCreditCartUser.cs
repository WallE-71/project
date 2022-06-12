using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;

namespace ShoppingStore.Application.Services
{
    public class ManageCreditCartUser : IManageCreditCartUser
    {
        private readonly IDataBaseContext _context;
        public ManageCreditCartUser(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<object> CreditCartAsync(string phoneNumber, int credit, string nationalId, string bankCode, bool getCart)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            var creditCarts = await _context.CreditCarts.FirstOrDefaultAsync(u => u.UserId == user.Id);
            if (creditCarts != null)
            {
                if (getCart) return creditCarts;
                creditCarts.Credit = credit;
                creditCarts.BankCode = bankCode;
                creditCarts.NationalId = nationalId;
                creditCarts.UpdateTime = DateTime.Now;
                creditCarts.UserId = user.Id;
                creditCarts.Owner = user.UserName;
                _context.CreditCarts.Update(creditCarts);
            }
            else
            {
                if (getCart) return null;
                var newCart = new CreditCart();
                newCart.Credit = credit;
                newCart.UserId = user.Id;
                newCart.BankCode = bankCode;
                newCart.Owner = user.UserName;
                newCart.NationalId = nationalId;
                newCart.InsertTime = DateTime.Now;
                await _context.CreditCarts.AddAsync(newCart);
            }
            await _context.SaveChangesAsync();
            return credit;
        }
    }
}
