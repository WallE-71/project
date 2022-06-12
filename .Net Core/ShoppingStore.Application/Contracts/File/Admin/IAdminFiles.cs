using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.File;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminFiles
    {
        Task InsertImagesAsync(string productId, int? sellerId, int? sliderId, int? userId, List<string> nameImages);
        Task UpdateImagesAsync(string productId, int? sellerId, int? sliderId, int? userId, List<string> nameImages);
        Task<List<FileViewModel>> GetPaginateFilesAsync(int offset, int limit, string Orderby, string searchText);
        Task<string> FindImageAsync(string productId, int? sellerId, int? sliderId, int? userId);
        Task<List<string>> GetImagesAsync(string productId, int? sellerId, int? sliderId);
    }
}
