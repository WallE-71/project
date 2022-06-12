using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Domain.Entities
{
    public class Seller : BaseEntity<int>
    {
        public string Email { get; set; }
        public string Brand { get; set; }
        public string SurName { get; set; }
        public string WebSite { get; set; }
        public string PhonNumber { get; set; }
        public string NationalId { get; set; }
        public DateTime? RegisterDate { get; set; }
        public ActivityType ActivityType { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }

    public enum ActivityType // نوع فعالیت
    {
        [Display(Name = "فروشنده")]
        Shopman = 1,

        [Display(Name = "تولید کننده")]
        Producer = 2,

        [Display(Name = "وارد کننده")]
        Importer = 3,

        [Display(Name = "همه موارد")]
        All = 4,
    }
}
