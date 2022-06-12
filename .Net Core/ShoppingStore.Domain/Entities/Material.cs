using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingStore.Domain.Entities
{
    public class Material : BaseEntity<int>
    {
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }
    }
}
