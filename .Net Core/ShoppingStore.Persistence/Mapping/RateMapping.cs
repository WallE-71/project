using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Domain.Mapping
{
    public class RateMapping : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.HasKey(r => new { r.UserId, r.SellerId });

            builder
              .HasOne(r => r.Seller)
              .WithMany(s => s.Ratings)
              .HasForeignKey(r => r.SellerId);
        }
    }
}
