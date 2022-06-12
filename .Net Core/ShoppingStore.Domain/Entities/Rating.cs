using System;
using System.ComponentModel.DataAnnotations;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Domain.Entities
{
    public class Rating
    {
        [Range(1, 5)]
        public double UserVote { get; set; }
        public DateTime? InsertTime { get; set; }

        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual int SellerId { get; set; }
        public virtual Seller Seller { get; set; }
    }
}
