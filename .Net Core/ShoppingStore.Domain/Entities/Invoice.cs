using System.Collections.Generic;

namespace ShoppingStore.Domain.Entities
{
    public class Invoice : BaseEntity<int>
    {
        public virtual ICollection<Order> Orders { get; set; }
    }
}
