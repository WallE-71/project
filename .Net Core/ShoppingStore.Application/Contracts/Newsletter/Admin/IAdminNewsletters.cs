using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.Newsletter;

namespace ShoppingStore.Application.Contracts
{
    public interface IAdminNewsletters
    {
        Task<List<NewsletterViewModel>> GetPaginateNewsletterAsync(int offset, int limit, string orderBy, string searchText);
        Task<Domain.Entities.Newsletter> FindByEmailAsync(string email);
    }
}
