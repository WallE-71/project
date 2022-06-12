using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.Manage
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password), Display(Name = "کلمه عبور فعلی"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password), Display(Name = "کلمه عبور جدید"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password), Display(Name = "تکرار کلمه عبور جدید"), Compare("NewPassword", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }
    }
}
