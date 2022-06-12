using System;
using System.Collections.Generic;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Application.ViewModels
{
    public class RequestPayViewModel
    {
        public class ItemRequestPay
        {
            public string Id { get; set; }
            public int Row { get; set; }
            public int Amount { get; set; }
            public string DisplayAmount { get; set; }
            public bool IsComplete { get; set; }
            public string PersianUpdateTime { get; set; }
        }

        public class ResultRequestPay
        {
            public string Id { get; set; }
            public int Amount { get; set; }
            public string Email { get; set; }
            public TransportType Transport { get; set; }
        }
    }
}
