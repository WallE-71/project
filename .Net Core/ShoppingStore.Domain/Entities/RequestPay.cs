using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Domain.Entities
{
    public class RequestPay : BaseEntity<string>
    {
        public int Amount { get; set; }
        public string Authority { get; set; }
        public long RefId { get; set; } = 0;
        public int? Tax { get; set; } = 9;
        public TransportType Transports { get; set; }
        public bool UseDiscount { get; set; }
        
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public enum TransportType
    {
        [Display(Name = "رایگان")]
        Free = 1,

        [Display(Name = "عادی")]
        Normal = 2,

        [Display(Name = "استانی")]
        Province = 3,

        [Display(Name = "ویژه")]
        Special = 4,
    }
}
