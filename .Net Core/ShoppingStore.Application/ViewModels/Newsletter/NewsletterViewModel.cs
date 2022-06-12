using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.Newsletter
{
    public class NewsletterViewModel : BaseViewModel<string>
    {
        [Display(Name = "ایمیل"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد.")]
        public string Email { get; set; }
    }
}
