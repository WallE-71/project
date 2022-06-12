using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Domain.Mapping
{
    public class BookmarkMapping : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.HasKey(b => new { b.UserId, b.ProductId });

            builder
              .HasOne(b => b.Product)
              .WithMany(p => p.Bookmarks)
              .HasForeignKey(b => b.ProductId);
        }
    }
}
