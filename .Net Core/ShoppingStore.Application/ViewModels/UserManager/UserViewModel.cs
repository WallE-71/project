using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Application.ViewModels.City;

namespace ShoppingStore.Application.ViewModels.UserManager
{
    public class UserViewModel
    {
        [JsonPropertyName("Id")]
        public int? Id { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [Display(Name = "شماره تماس"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمی باشد.")]
        public string Email { get; set; }
        ///\\\///\\\///\\\///\\\///\\\///\\\///\\\

        [Display(Name = "تصویر"), JsonIgnore]
        public string Image { get; set; }

        [Display(Name = "تصویر پروفایل"), Required(ErrorMessage = "انتخاب {0} الزامی است."), JsonIgnore]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "نام کاربری"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string UserName { get; set; }

        [Display(Name = "کلمه عبور"), DataType(DataType.Password), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور"), DataType(DataType.Password), Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد."), JsonIgnore]
        public string ConfirmPassword { get; set; }

        [Display(Name = "نام"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string LastName { get; set; }
        ///\\\///\\\///\\\///\\\///\\\///\\\///\\\

        [Display(Name = "تاریخ تولد"), JsonIgnore]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "تاریخ تولد"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), JsonIgnore]
        public string PersianBirthDate { get; set; }

        [Display(Name = "تاریخ عضویت"), JsonIgnore]
        public DateTime? InsertTime { get; set; } = DateTime.Now;

        [Display(Name = "تاریخ عضویت")]
        public string PersianInsertTime { get; set; }

        [Display(Name = "جنسیت")]
        public string GenderName { get; set; }

        [Display(Name = "جنسیت"), Required(ErrorMessage = "انتخاب {0} الزامی است."), JsonIgnore]
        public GenderType? Gender { get; set; }

        [Display(Name = "آدرس"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Address { get; set; }

        [Display(Name = "معرفی")]
        public string Bio { get; set; }
             
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        [JsonIgnore]
        public ICollection<FileStore> FileStores { get; set; }

        /// <summary>
        /// Settings User
        /// </summary>
        [JsonIgnore]
        public bool PhoneNumberConfirmed { get; set; }

        [JsonIgnore]
        public bool TwoFactorEnabled { get; set; }

        [JsonIgnore]
        public bool LockoutEnabled { get; set; }

        [JsonIgnore]
        public bool EmailConfirmed { get; set; }

        [JsonIgnore]
        public int AccessFailedCount { get; set; }

        [JsonIgnore]
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        [Display(Name = "نقش"), Required(ErrorMessage = "انتخاب {0} الزامی است."), JsonIgnore]
        public long RoleId { get; set; }
        public List<int> RolesId { get; set; } = new List<int>();

        public string RoleName { get; set; }
        public List<string> RolesName { get; set; } = new List<string>();

        [JsonIgnore]
        public int[] RoleIds { get; set; }

        [JsonIgnore]
        public int IdOfRole { get; set; }

        [JsonIgnore]
        public int[] IdOfRoles { get; set; }

        [JsonIgnore]
        public ICollection<UserRole> Roles { get; set; }

        [JsonIgnore]
        public ListOfRolesViewModel ListOfRoles { get; set; }
    
        /// <summary>
        /// City - Province
        /// </summary>
        [JsonIgnore]
        public int? CityId { get; set; }

        [JsonIgnore]
        public List<TreeViewCity> Cities { get; set; }

        public string CityName { get; set; }

        public string ProvinceName { get; set; }
        ///\\\///\\\///\\\///\\\///\\\///\\\///\\\
    }
}
