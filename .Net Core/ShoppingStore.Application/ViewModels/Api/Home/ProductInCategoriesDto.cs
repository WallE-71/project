using System;

namespace ShoppingStore.Application.ViewModels.Api.Home
{
    public  class ProductInCategoriesDto : BaseViewModel<string>
    {
        public long Price { get; set; }
        public string ImageName { get; set; }
        public string NameOfBrand { get; set; }
        public string ActionDiscount { get; set; }
        public string DisplayDiscount { get; set; }
        public double PercentDiscount { get; set; }
        public string NameOfCategories { get; set; }
        public string PersianExpirationDate { get; set; }

        public double BestOfSellers { get; set; }

        public int NumberOfLike { get; set; }
        public int? NumberOfSale { get; set; }
        public int NumberOfVisit { get; set; }
        public int NumberOfDisLike { get; set; }
        public int NumberOfComments { get; set; }
    }
}
