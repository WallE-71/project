using ShoppingStore.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Domain.Entities
{
    public class MessageUser : BaseEntity<int>
    {
        public string Email { get; set; }
        public string Answer { get; set; }
        public MessageType Messages { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
    }

    public enum MessageType
    {
        [Display(Name = "تیکت")]
        Tiket = 1,

        [Display(Name = "درخواست عضویت بعنوان فروشنده‌")]
        RequestRegister = 2,

        [Display(Name = "انتقادات و پیشنهادات")]
        FeedbackOffers = 3,

        [Display(Name = "سوالات")]
        Questions = 4
    }
}
