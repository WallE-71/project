using System.Collections.Generic;

namespace ShoppingStore.Domain.Entities
{
    public class Color : BaseEntity<int>
    {
        public virtual ICollection<ProductColor> ProductColors { get; set; }
    }
}
