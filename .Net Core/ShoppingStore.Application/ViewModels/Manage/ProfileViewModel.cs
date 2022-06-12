using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Application.ViewModels.Manage
{
    public class ProfileViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "تصویر پروفایل")]
        public string Image { get; set; }

        [Display(Name = "تصویر پروفایل")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "نام کاربری"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمی باشد.")]
        public string Email { get; set; }

        [Display(Name = "شماره موبایل"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "نام"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string LastName { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "تاریخ تولد"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PersianBirthDate { get; set; }

        [Display(Name = "جنسیت"), Required(ErrorMessage = "انتخاب {0} الزامی است.")] // به شرط نال پذیر بودن پراپرتی استفاده کرد Required() از اتریبیوت RadioButton می توان برای اعتبارسنجی 
        public GenderType? Gender { get; set; } // کاربر حتما باید جنسیت خود را انتخاب کند وگرنه پیغام خطا نمایش داده می شود و علامت سوال اگر نباشد و کاربر جنسیت خود را انتخاب نکند مقدار پیش فرض برای او در نظر گرفته می شود و پیغام خطا نمایش داده نمی شود

        [Display(Name = "معرفی")]
        public string Bio { get; set; }
    }
}
