using System.Collections.Generic;
using ShoppingStore.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Domain.Entities
{
    public class Order : BaseEntity<int>
    {
        public long AmountPaid { get; set; }
        public int Quantity { get; set; }
        public OrderState States { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string RequestPayId { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual RequestPay RequestPay { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
    }

    public enum OrderState
    {
        [Display(Name = "در حال پردازش")]
        Processing = 1,

        [Display(Name = "تایید شده")]
        Confirmed = 2,

        [Display(Name = "در حال ارسال")]
        Transmission = 3,

        [Display(Name = "آماده تحویل")]
        Delivered = 4,
    }
}
