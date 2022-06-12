using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Domain.Mapping
{
    public class ProductColorMapping : IEntityTypeConfiguration<ProductColor>
    {
        public void Configure(EntityTypeBuilder<ProductColor> builder)
        {
            builder.HasKey(pc => new { pc.ProductId, pc.ColorId });
            builder
              .HasOne(pc => pc.Product)
              .WithMany(p => p.ProductColors)
              .HasForeignKey(pc => pc.ProductId);

            builder
               .HasOne(pc => pc.Color)
               .WithMany(c => c.ProductColors)
               .HasForeignKey(pc => pc.ColorId);
        }
    }
}
