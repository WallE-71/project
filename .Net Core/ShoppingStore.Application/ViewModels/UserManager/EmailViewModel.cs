using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.UserManager
{
    public class EmailViewModel
    {
        [JsonPropertyName("Id")]
        public int? Id { get; set; }

        [Display(Name = "ایمیل قبلی"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمی باشد.")]
        public string OldEmail { get; set; }

        [Display(Name = "ایمیل جدید"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمی باشد.")]
        public string NewEmail { get; set; }
    }
}
