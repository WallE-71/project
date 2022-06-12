using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Domain.Entities
{
    public class Discount
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ProductId { get; set; }
        public double Percent { get; set; }
        public string DiscountCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DiscountDay { get; set; }

        public DateTime? InsertTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
