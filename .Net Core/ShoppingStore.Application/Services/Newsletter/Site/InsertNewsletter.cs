using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;

namespace ShoppingStore.Application.Services
{
    public class InsertNewsletter : IInsertNewsletter
    {
        private readonly IDataBaseContext _context;
        public InsertNewsletter(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<string> SendMessage(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return "ایمیل شما معتبر نیست, ابتدا وارد شوید!";
            if (user.IsActive == true)
            {
                int maxId;
                var newsletter = await _context.Newsletters.FirstOrDefaultAsync(u => u.Email == email);
                if (newsletter == null)
                    newsletter = new Newsletter();
                else
                {
                    newsletter.UpdateTime = DateTime.Now;
                    _context.Newsletters.Update(newsletter);
                    await _context.SaveChangesAsync();
                    return "شما عضو خبرنامه هستید.";
                }

                var newsletters = await _context.Newsletters.AsNoTracking().ToListAsync();
                if (newsletters != null)
                    maxId = newsletters.OrderByDescending(c => c.Id).First().Id + 1;
                else
                    maxId = 1;
                newsletter.Id = maxId;
                newsletter.Email = email;
                newsletter.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd");
                await _context.Newsletters.AddAsync(newsletter);
                await _context.SaveChangesAsync();
                return "عضویت شما در خبرنامه با موفقیت انجام شد.";
            }
            else
                return "حساب کاربری شما غیر فعال است!";
        }
    }
}
