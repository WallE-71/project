using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Domain.Mapping
{
    public class OrderMapping : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(o => o.User)
                .WithMany(o => o.Orders)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(o => o.RequestPay)
                .WithMany(o => o.Orders)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(o => o.Invoice)
                .WithMany(o => o.Orders)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
