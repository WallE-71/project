using Microsoft.EntityFrameworkCore;

namespace ShoppingStore.Domain.Mapping
{
    public static class ProjectMapping
    {
        public static void AddCustomProjectMappings(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RateMapping());
            modelBuilder.ApplyConfiguration(new LikeMapping());
            modelBuilder.ApplyConfiguration(new VisitMapping());
            modelBuilder.ApplyConfiguration(new OrderMapping());
            modelBuilder.ApplyConfiguration(new CategoryMapping());     
            modelBuilder.ApplyConfiguration(new BookmarkMapping());
            modelBuilder.ApplyConfiguration(new ProductColorMapping());
            modelBuilder.ApplyConfiguration(new ProductCategoryMapping());
            modelBuilder.ApplyConfiguration(new ProductMaterialMapping());
        }
    }
}
