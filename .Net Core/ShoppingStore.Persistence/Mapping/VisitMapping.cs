using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Domain.Mapping
{
    public class VisitMapping : IEntityTypeConfiguration<Visit>
    {
        public void Configure(EntityTypeBuilder<Visit> builder)
        {
            builder.HasKey(v => new { v.ProductId, v.IpAddress });
            builder
              .HasOne(v => v.Product)
              .WithMany(p => p.Visits)
              .HasForeignKey(v => v.ProductId);
        }
    }
}
