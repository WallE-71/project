using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.UserManager
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور جدید"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}
