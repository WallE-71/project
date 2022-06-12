using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Application.ViewModels.Product
{
    public class DiscountViewModel
    {
        public string ProductId { get; set; }

        [JsonPropertyName("تاریخ شروع تخفیف"), Display(Name = "تاریخ شروع تخفیف"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PersianStartDate { get; set; }
        public DateTime StartDate { get; set; }

        [JsonPropertyName("تاریخ پایان"), Display(Name = "تاریخ پایان"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PersianEndDate { get; set; }
        public DateTime EndDate { get; set; }

        [JsonPropertyName("درصد تخفیف"), Display(Name = "درصد تخفیف"), Range(0,100), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public double Percent { get; set; }

        [Display(Name = "کد تحفیف")]
        public string DiscountCode { get; set; }
        public string DiscountDay { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? InsertTime { get; set; }      
    }
}
