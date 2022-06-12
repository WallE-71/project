using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Application.ViewModels.Seller
{
    public class SellerViewModel : BaseViewModel<int>
    {
        [Display(Name = "تصویر فروشنده")]
        public string SellerImage { get; set; }

        [Display(Name = "تصویر فروشنده"), Required(ErrorMessage = "انتخاب {0} الزامی است."), JsonIgnore]
        public IFormFile SellerImageFile { get; set; }

        [Display(Name = "تصویر کارت ملی")]
        public string NationalIdImage { get; set; }

        [Display(Name = "تصویر کارت ملی"), Required(ErrorMessage = "انتخاب {0} الزامی است."), JsonIgnore]
        public IFormFile NationalIdImageFile { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string SurName { get; set; }

        [Display(Name = "کد ملی")]
        public string NationalId { get; set; }

        [Display(Name = "شماره موبایل")]
        public string PhonNumber { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "نام برند")]
        public string Brand { get; set; }

        [Display(Name = "وب سایت")]
        public string WebSite { get; set; }

        [Display(Name = "نوع فعالیت")]
        public ActivityType ActivityType { get; set; }

        public DateTime? RegisterDate { get; set; }

        [Display(Name = "تاریخ عضویت")]
        public string PersianRegisterDate { get; set; }

        public double UserVote { get; set; }
        public int NumberOfProducts { get; set; }

        // Store
        [Display(Name = "نام فروشگاه یا شرکت")]
        public string Store { get; set; }

        [Display(Name = "شماره ثابت محل کار")]
        public string TelNumber { get; set; }

        [Display(Name = "کد پستی"), Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public string PostalCode { get; set; }

        [Display(Name = "آدزس"), Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public string Address { get; set; }

        public DateTime? EstablishmentDate { get; set; }

        [Display(Name = "تاریخ تاسیس")]
        public string PersianEstablishmentDate { get; set; }

        [Display(Name = "محصولات نمونه")]
        public string SampleProduct { get; set; }

        [Display(Name = "تصویر سند")]
        public string DocumentImage { get; set; }

        [Display(Name = "تصویر سند"), Required(ErrorMessage = "انتخاب {0} الزامی است."), JsonIgnore]
        public string DocumentImageFile { get; set; }

        public string HealthBarcodeImage { get; set; }
        public bool IsConfirmDocuments { get; set; }
    }
}
