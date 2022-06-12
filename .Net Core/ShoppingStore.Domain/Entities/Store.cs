using System;

namespace ShoppingStore.Domain.Entities
{
    public class Store : BaseEntity<int>
    {
        public string TelNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public long RegisterCode { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        public string SupportLocation { get; set; }

        public int SellerId { get; set; }
        public int? CityId { get; set; }
    }
}
