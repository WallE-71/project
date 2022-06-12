using System;

namespace ShoppingStore.Domain.Entities
{
    public class Advertising : BaseEntity<int>
    {
        public string Url { get; set; }
        public ImageLocation ImageLocation { get; set; }

        public int SliderId { get; set; }
    }
}
