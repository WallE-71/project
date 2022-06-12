using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingStore.Domain.Entities
{
    public class ProductColor
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }
    }
}
