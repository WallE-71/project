using System.Collections.Generic;

namespace ShoppingStore.Domain.Entities
{
    public class Brand : BaseEntity<int>
    {
        public virtual List<Product> Products { get; set; }
    }
}
