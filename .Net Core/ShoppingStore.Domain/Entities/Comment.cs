using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Domain.Entities
{
    public class Comment : BaseEntity<int>
    {
        public Comment()
        {
            SubComments = new List<Comment>();
        }

        public string Email { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        public virtual Comment Parent { get; set; }
        public virtual ICollection<Comment> SubComments { get; set; }

        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
