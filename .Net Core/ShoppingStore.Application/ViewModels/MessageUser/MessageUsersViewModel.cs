using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Application.ViewModels.MessageUser
{
    public class MessageUsersViewModel : BaseViewModel<int>
    {
        [JsonIgnore]
        public long UserId { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "نوع پیام")]
        public MessageType Messages { get; set; }

        [Display(Name = "نوع پیام")]
        public string Message { get; set; }

        [Display(Name = "متن پاسخ"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Answer { get; set; }

        [Display(Name = "پاسخ دهنده")]
        public string AnswerAuthor { get; set; }

        [JsonIgnore]
        public string MessageTargetRole { get; set; }
    }
}
