using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Application.Contracts
{
    public interface IDataBaseContext
    {
        DbSet<User> Users { set; get; }
        DbSet<Role> Roles { set; get; }
        DbSet<UserRole> UserRoles { set; get; }
        DbSet<UserClaim> UserClaims { set; get; }
        DbSet<RoleClaim> RoleClaims { set; get; }

        DbSet<Cart> Carts { get; set; }
        DbSet<Like> Likes { set; get; }
        DbSet<City> Cities { set; get; }
        DbSet<Brand> Brands { set; get; }
        DbSet<Color> Colors { set; get; }
        DbSet<Order> Orders { set; get; }
        DbSet<Store> Stores { set; get; }
        DbSet<Visit> Visits { set; get; }
        DbSet<Rating> Ratings { set; get; }
        DbSet<Slider> Sliders { set; get; }
        DbSet<Seller> Sellers { set; get; }
        DbSet<Invoice> Invoices { set; get; }
        DbSet<Comment> Comments { set; get; }
        DbSet<Product> Products { set; get; }
        DbSet<Bookmark> Bookmarks { set; get; }
        DbSet<Discount> Discounts { set; get; }
        DbSet<Province> Provinces { set; get; }
        DbSet<Material> Materials { set; get; }
        DbSet<CartItem> CartItems { set; get; }
        DbSet<Category> Categories { set; get; }
        DbSet<Guarantee> Guarantees { set; get; }
        DbSet<FileStore> FileStores { set; get; }
        DbSet<CreditCart> CreditCarts { set; get; }
        DbSet<Newsletter> Newsletters { set; get; }
        DbSet<RequestPay> RequestPays { set; get; }
        DbSet<Advertising> Advertisings { set; get; }
        DbSet<MessageUser> MessageUsers { set; get; }
        DbSet<OrderDetail> OrderDetails { set; get; }
        DbSet<ProductColor> ProductColors { set; get; }
        DbSet<ProductMaterial> ProductMaterials { set; get; }
        DbSet<ProductCategory> ProductCategories { set; get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(object entity) where TEntity : class;

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
    }
}
