using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Domain.Entities
{
    public class City : BaseEntity<int>
    {
        [ForeignKey("Province")]
        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; }
    }

    public class Province : BaseEntity<int>
    {
        public virtual List<City> Cities { get; set; }
    }
}
