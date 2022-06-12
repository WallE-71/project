using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Domain.Entities
{
    public class BaseEntity<TKey>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public TKey Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; } = false;
        public DateTime? RemoveTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? InsertTime { get; set; }
    }
}
