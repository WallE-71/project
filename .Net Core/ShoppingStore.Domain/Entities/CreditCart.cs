using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Domain.Entities
{
    public class CreditCart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Credit { get; set; }
        public string Owner { get; set; }
        public string NationalId { get; set; }
        public string BankCode { get; set; }
        public DateTime? RemoveTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? InsertTime { get; set; }
        
        public virtual int? UserId { get; set; }
        public virtual int? SellerId { get; set; }
    }
}
