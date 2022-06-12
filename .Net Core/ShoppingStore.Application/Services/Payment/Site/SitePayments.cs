using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels;
using ShoppingStore.Domain.Common.Extensions;
using static ShoppingStore.Application.ViewModels.RequestPayViewModel;

namespace ShoppingStore.Application.Services
{
    public class SitePayments : ISitePayments
    {
        private readonly IDataBaseContext _context;
        public SitePayments(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<ResultViewModel<ResultRequestPay>> AddRequestPayAsync(int amount, int userId, TransportType transportType, string discountCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var discount = await _context.Discounts.FirstOrDefaultAsync(p => p.DiscountCode == discountCode);
            var transportPrice = GetTransportType(transportType);
            amount = CalculateAmount(amount, transportPrice, discount);

            var findRequestPay = await _context.RequestPays.Where(r => r.UserId == userId && r.IsComplete == false).FirstOrDefaultAsync();
            if (findRequestPay != null)
            {
                findRequestPay.Tax = 9;
                findRequestPay.Amount = amount;
                findRequestPay.UpdateTime = findRequestPay.IsComplete ? findRequestPay.UpdateTime = DateTime.Now : null;
                _context.RequestPays.Update(findRequestPay);
                await _context.SaveChangesAsync();

                return new ResultViewModel<ResultRequestPay>()
                {
                    Data = new ResultRequestPay
                    {
                        Email = user.Email,
                        Id = findRequestPay.Id,
                        Amount = findRequestPay.Amount,
                    },
                    IsSuccess = true,
                };
            }
            var requestPay = new RequestPay()
            {
                Tax = 9,
                User = user,
                Amount = amount,
                Transports = transportType,
                Id = StringExtensions.GenerateId(10),
                UseDiscount = discount == null ? false : true,
            };
            await _context.RequestPays.AddAsync(requestPay);
            await _context.SaveChangesAsync();

            return new ResultViewModel<ResultRequestPay>()
            {
                Data = new ResultRequestPay
                {
                    Id = requestPay.Id,
                    Email = user.Email,
                    Transport = transportType,
                    Amount = requestPay.Amount,
                },
                IsSuccess = true,
            };
        }

        public async Task<ResultViewModel<ItemRequestPay>> GetRequestPayAsync(string requestPayId)
        {
            var requestPay = await _context.RequestPays.Where(p => p.Id == requestPayId).FirstOrDefaultAsync();
            if (requestPay != null)
            {
                return new ResultViewModel<ItemRequestPay>()
                {
                    Data = new ItemRequestPay()
                    {
                        Id = requestPay.Id,
                        Amount = requestPay.Amount,
                        DisplayAmount = requestPay.Amount.ToString().En2Fa(),
                    }
                };
            }
            else
                throw new Exception("request pay not found");
        }

        public int GetTransportType(TransportType transportType)
        {
            return transportType == TransportType.Free ? 0 :
                        transportType == TransportType.Normal ? 10000 :
                        transportType == TransportType.Province ? 30000 : 50000;
        }

        public int CalculateAmount(int amount, int transportPrice, Discount discount)
        {
            double _amount, removeDecimal;
            double.TryParse(amount.ToString(), out _amount);
            var truncate = Math.Truncate(_amount / 9);
            double.TryParse(truncate.ToString(), out removeDecimal);
            amount += Convert.ToInt32(removeDecimal) + transportPrice;
            amount *= (discount == null ? 1 : (int)discount.Percent / 100);
            return amount;
        }
    }
}
