using System.Collections.Generic;
using ShoppingStore.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Domain.Entities
{
    public class Cart : BaseEntity<int>
    {
        public string BrowserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }     
    }
}
