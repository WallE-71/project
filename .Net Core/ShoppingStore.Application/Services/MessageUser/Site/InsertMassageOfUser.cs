using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;

namespace ShoppingStore.Application.Services
{
    public class InsertMassageOfUser : IInsertMassageOfUser
    {
        private readonly IDataBaseContext _context;
        public InsertMassageOfUser(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<string> SendMessage(string email, string description, int typeFeedBack)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return "ایمیل شما معتبر نیست!";
            if (user.IsActive == true && user.IsValidAccount == true)
            {
                var messageUser = await _context.MessageUsers.FirstOrDefaultAsync(m => m.UserId == user.Id);
                if (messageUser == null)
                {
                    int maxId;
                    messageUser = new MessageUser();
                    var messageUsers = await _context.MessageUsers.AsNoTracking().ToListAsync();
                    if (messageUsers.Count() != 0)
                        maxId = messageUsers.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                    else
                        maxId = 1;
                    messageUser.Id = maxId;
                    messageUser.Email = email;
                    messageUser.UserId = user.Id;
                    messageUser.Description = description;
                    messageUser.Messages = (MessageType)typeFeedBack;
                    messageUser.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _context.MessageUsers.AddAsync(messageUser);
                }
                else
                {
                    messageUser.UpdateTime = DateTime.Now;
                    messageUser.Description = description;
                    messageUser.Messages = (MessageType)typeFeedBack;
                    _context.MessageUsers.Update(messageUser);
                }
                await _context.SaveChangesAsync();
                return email;
            }
            else
                return "حساب کاربری شما غیر فعال است!";
        }
    }
}
