using System;

namespace ShoppingStore.Application.Contracts
{
    public interface IUnitOfWork
    {
        IBaseService<TEntity> BaseService<TEntity>() where TEntity : class;
        IAdminBrands AdminBrands { get; }
        IAdminCarts AdminCarts { get; }
        ISiteCarts SiteCarts { get; }
        IManageShoppingCart ManageShoppingCart { get; }
        IAdminCategories AdminCategories { get; }
        ISiteCategories SiteCategories { get; }
        IAdminCities AdminCities { get; }
        IAdminColors AdminColors { get; }
        IAdminComments AdminComments { get; }
        ISiteComments SiteComments { get; }
        IInsertCommentUser InsertCommentUser { get; }
        IAdminCreditCarts AdminCreditCarts { get; }
        IManageCreditCartUser ManageCreditCartUser { get; }
        IAdminFiles AdminFiles { get; }
        IInsertLikeOrDislike InsertLikeOrDislike { get; }
        IAdminMaterials AdminMaterials { get; }
        IAdminMessageUsers AdminMessageUsers { get; }
        IInsertMassageOfUser InsertMassageOfUser { get; }
        IAdminNewsletters AdminNewsletters { get; }
        IInsertNewsletter InsertNewsletter { get; }
        IAdminOrders AdminOrders { get; }
        ISiteOrders SiteOrders { get; }
        IAdminPayments AdminPayments { get; }
        ISitePayments SitePayments { get; }
        IAdminProducts AdminProducts { get; }
        IAdminProvinces AdminProvinces { get; }
        ISiteProducts SiteProducts { get; }
        IInsertRateSaller InsertRateSaller { get; }
        ISiteSellers SiteSellers { get; }
        IAdminSellers AdminSellers { get; }
        IInsertRequestSeller InsertRequestSeller { get; }
        IAdminSliders AdminSliders { get; }
        ISiteSliders SiteSliders { get; }
        IAdminUsers AdminUsers { get; }
    }
}
