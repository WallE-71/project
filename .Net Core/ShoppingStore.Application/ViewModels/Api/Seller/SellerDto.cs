using System;

namespace ShoppingStore.Application.ViewModels.Api.Seller
{
    public class SellerDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Brand { get; set; }
        public string SurName { get; set; }
        public string WebSite { get; set; }
        public int ActivityType { get; set; }
        public string ImageFile { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string ScanDocument { get; set; }
        public string ScanNationalIdCart { get; set; }

        public string Store { get; set; }
        public string Address { get; set; }
        public string TelNumber { get; set; }
        public string PostalCode { get; set; }
        public string SampleProduct { get; set; }
        public string EstablishmentDate { get; set; }
    }
}
