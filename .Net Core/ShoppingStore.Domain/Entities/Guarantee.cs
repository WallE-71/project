using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingStore.Domain.Entities
{
    public class Guarantee : BaseEntity<int>
    {
        public virtual List<Product> Products { get; set; }
    }
}
