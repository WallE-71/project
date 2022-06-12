using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.Manage
{
    public class RegisterViewModel
    {
        [Display(Name = "نام کاربری"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمی باشد.")]
        public string Email { get; set; }

        [DataType(DataType.Password), Display(Name = "کلمه عبور"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password), Display(Name = "تکرار کلمه عبور"), Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }
    }
}
