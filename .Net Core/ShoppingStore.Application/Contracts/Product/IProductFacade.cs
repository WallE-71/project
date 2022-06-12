using System;

namespace ShoppingStore.Application.Contracts
{
    public interface IProductFacade
    {
        ISiteProducts SiteProducts { get; }
        IAdminProducts AdminProducts { get; }
        //IProductService ProductService { get; }
    }
}
