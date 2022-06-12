using System;

namespace ShoppingStore.Domain.Entities
{
    public class CartItem : BaseEntity<int>
    {
        public int Count { get; set; }
        public int Price { get; set; }

        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
        public string ProductId { get; set; }
        public string ProductColor { get; set; }
        public virtual Product Product { get; set; }
    }
}
