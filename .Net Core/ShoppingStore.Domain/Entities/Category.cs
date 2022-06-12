using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Domain.Entities
{
    public class Category : BaseEntity<int>
    {
        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        public virtual Category Parent { get; set; }
        public virtual List<Category> SubCategories { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
