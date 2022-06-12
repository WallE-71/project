using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Product;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminProducts
    {
        Task<List<ProductViewModel>> GetPaginateProductAsync(int offset, int limit, string orderBy, string searchText, bool? IsDemo, bool? isSeller, long sellerId = 0);
        Task<int> GetNumberOfVisitAsync(DateTime EndDateTimeMiladi, DateTime StartDateTimeMiladi);
        Task<List<ProductViewModel>> ProductDetailFeatures(string productId);
        Task<List<ProductViewModel>> FindProductByIdAsync(string productId);
        Task<int> InsertProductGuarantee(string guaranteeName);

        int CountProductSale();
        int CountProductPresentation();

        List<string> GetBrands();
        List<string> GetColors();
        List<string> GetMaterials();
        List<string> GetGuarantees();
    }
}
