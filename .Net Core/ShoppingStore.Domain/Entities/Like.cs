using System;
using System.Text;
using System.Collections.Generic;

namespace ShoppingStore.Domain.Entities
{
    public class Like
    {
        public string BrowserId { get; set; }
        public bool IsLiked { get; set; }

        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
