using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ShoppingStore.Domain.Mapping;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Persistence
{
    public class DataBaseContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, IdentityUserLogin<int>, RoleClaim, IdentityUserToken<int>>, IDataBaseContext
    {
        public DataBaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
            OtherSettings(modelBuilder);
            FilterRemovedEntities(modelBuilder);
            modelBuilder.AddCustomProjectMappings();
            modelBuilder.AddCustomIdentityMappings();
        }

        public virtual DbSet<Cart> Carts { set; get; }
        public virtual DbSet<Like> Likes { set; get; }
        public virtual DbSet<City> Cities { set; get; }
        public virtual DbSet<Brand> Brands { set; get; }
        public virtual DbSet<Color> Colors { set; get; }
        public virtual DbSet<Order> Orders { set; get; }
        public virtual DbSet<Store> Stores { set; get; }
        public virtual DbSet<Visit> Visits { set; get; }
        public virtual DbSet<Rating> Ratings { set; get; }
        public virtual DbSet<Slider> Sliders { set; get; }
        public virtual DbSet<Seller> Sellers { set; get; }
        public virtual DbSet<Invoice> Invoices { set; get; }
        public virtual DbSet<Comment> Comments { set; get; }
        public virtual DbSet<Product> Products { set; get; }
        public virtual DbSet<Bookmark> Bookmarks { set; get; }
        public virtual DbSet<Discount> Discounts { set; get; }
        public virtual DbSet<Province> Provinces { set; get; }
        public virtual DbSet<Material> Materials { set; get; }
        public virtual DbSet<CartItem> CartItems { set; get; }
        public virtual DbSet<Category> Categories { set; get; }
        public virtual DbSet<Guarantee> Guarantees { set; get; }
        public virtual DbSet<FileStore> FileStores { set; get; }
        public virtual DbSet<CreditCart> CreditCarts { set; get; }
        public virtual DbSet<Newsletter> Newsletters { set; get; }
        public virtual DbSet<RequestPay> RequestPays { set; get; }
        public virtual DbSet<Advertising> Advertisings { set; get; }
        public virtual DbSet<MessageUser> MessageUsers { set; get; }
        public virtual DbSet<OrderDetail> OrderDetails { set; get; }
        public virtual DbSet<ProductColor> ProductColors { set; get; }
        public virtual DbSet<ProductMaterial> ProductMaterials { set; get; }
        public virtual DbSet<ProductCategory> ProductCategories { set; get; }

        private void OtherSettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Seller>().HasIndex(u => u.NationalId).IsUnique();
            modelBuilder.Entity<Seller>().HasIndex(u => u.PhonNumber).IsUnique();
            modelBuilder.Entity<Newsletter>().Property(b => b.IsComplete).HasDefaultValueSql("1");
            modelBuilder.Entity<Newsletter>().Property(b => b.InsertTime).HasDefaultValueSql("CONVERT(DATETIME, CONVERT(VARCHAR(20),GetDate(), 120))");
        }

        private void FilterRemovedEntities(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasQueryFilter(u => u.RemoveTime == null);
            //modelBuilder.Entity<Role>().HasQueryFilter(r => r.RemoveTime == null);
            //modelBuilder.Entity<Cart>().HasQueryFilter(c => c.RemoveTime == null);
            //modelBuilder.Entity<Store>().HasQueryFilter(s => s.RemoveTime == null);
            //modelBuilder.Entity<Order>().HasQueryFilter(o => o.RemoveTime == null);
            //modelBuilder.Entity<Seller>().HasQueryFilter(s => s.RemoveTime == null);
            //modelBuilder.Entity<Comment>().HasQueryFilter(c => c.RemoveTime == null);
            //modelBuilder.Entity<Product>().HasQueryFilter(p => p.RemoveTime == null);
            //modelBuilder.Entity<Category>().HasQueryFilter(c => c.RemoveTime == null);
            //modelBuilder.Entity<CartItem>().HasQueryFilter(c => c.RemoveTime == null);
            //modelBuilder.Entity<FileStore>().HasQueryFilter(f => f.RemoveTime == null);
            //modelBuilder.Entity<CreditCart>().HasQueryFilter(c => c.RemoveTime == null);
            //modelBuilder.Entity<RequestPay>().HasQueryFilter(r => r.RemoveTime == null);
            //modelBuilder.Entity<Newsletter>().HasQueryFilter(n => n.RemoveTime == null);
            //modelBuilder.Entity<MessageUser>().HasQueryFilter(m => m.RemoveTime == null);
            //modelBuilder.Entity<Discount>().HasQueryFilter(d => d.EndDate > System.DateTime.Now);
        }

        public EntityEntry<TEntity> Entry<TEntity>(object entity) where TEntity : class
        {
            throw new System.NotImplementedException();
        }
    }
}
