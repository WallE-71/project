using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Product;
using ShoppingStore.Application.ViewModels.Api.Home;

namespace ShoppingStore.Application.Contracts
{
    public interface ISiteProducts
    {
        Task<List<ProductInCategoriesDto>> GetProductInCategoryAsync(string categoryName, string filter, int checkedDiscount, string typeDiscount);
        Task<ProductViewModel> GetProductByIdAsync(string productId, string userId);

        Task<List<ProductInCategoriesDto>> GetRelatedProductAsync(int number, List<int> categoryIdList, string productId);
        Task<List<ProductInCategoriesDto>> SpecialDiscountProductsAsync(string day);
        Task<List<ProductInCategoriesDto>> GetPreferedProductsNoDiscountAsync();
        Task<List<ProductInCategoriesDto>> MostDiscountProductAsync();
        Task<List<ProductInCategoriesDto>> GetPreferedProductsAsync();
        Task<List<ProductInCategoriesDto>> MostPopularProductsAsync();
        Task<List<ProductInCategoriesDto>> MostViewedProductAsync();
        Task<List<ProductInCategoriesDto>> MostTalkProductAsync();

        Task InsertVisitOfUserAsync(string productId, Guid? browserId, string ipAddress);
        Task<string> GetWeeklyProductDiscountAsync(string Url);
    }
}
