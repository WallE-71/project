using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Application.ViewModels.Order
{
    public class OrderViewModel : BaseViewModel<int>
    {
        [Display(Name = "تعداد")]
        public int Quantity { get; set; } 
        
        [Display(Name = "تعداد")]
        public string StringQuantity { get; set; } 

        [Display(Name = "مبلغ پرداخت شده"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public long AmountPaid { get; set; }

        [Display(Name = "مبلغ پرداخت شده")]
        public string StringAmountPaid { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string RequestPayId { get; set; }

        [Display(Name = "وضعیت"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public OrderState OrderState { get; set; }

        [Display(Name = "وضعیت")]
        public string Status { get; set; }

        [Display(Name = "مجموع سبد خرید")]
        public string TotalCart { get; set; }

        [Display(Name = "نام مشتری")]
        public string CustomerName { get; set; }

        [Display(Name = "نام محصول سفارشی")]
        public string ProductName { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetailViewModel> OrderDetails { get; set; }
    }

    public class OrderDetailViewModel
    {
        [Display(Name = "قیمت")]
        public int Price { get; set; }

        [Display(Name = "تعداد")]
        public int Count { get; set; }

        [JsonIgnore]
        public string ProductId { get; set; }

        [Display(Name = "نام محصول سفارشی")]
        public string ProductName { get; set; }

        //[JsonIgnore]
        public string ImageName { get; set; }

        [JsonIgnore]
        public int OrderDetailId { get; set; }
    }

    public class UserOrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "وضعیت")]

        public int Row { get; set; } = 1;
        public OrderState States { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string RequestPayId { get; set; }
        public string Customer { get; set; }
        public long AmountPaid { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Deadline { get; set; }
        public string PersianInsertTime { get; set; }
        public string PersianDeadline { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }
 
    public class RequestAddNewOrder
    {
        public int UserId { get; set; }
        public long RefId { get; set; } = 0;
        public int CartId { get; set; }
        public string Authority { get; set; }
        public string RequestPayId { get; set; }
        public string DispatchNumber { get; set; }
    }
}
