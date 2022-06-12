using System;

namespace ShoppingStore.Domain.Entities
{
    public class Visit
    {
        public string IpAddress { get; set; }
        public Guid? BrowserId { get; set; }
        public int NumberOfVisit { get; set; }
        public DateTime LastVisitDateTime { get; set; }

        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
