using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Domain.Mapping
{
    public static class IdentityMapping
    {
        public static void AddCustomIdentityMappings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("AppUsers");
            modelBuilder.Entity<Role>().ToTable("AppRoles");
            modelBuilder.Entity<UserRole>().ToTable("AppUserRole");
            modelBuilder.Entity<RoleClaim>().ToTable("AppRoleClaim");
            modelBuilder.Entity<UserClaim>().ToTable("AppUserClaim");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("AppUserLogin");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("AppUserToken");

            modelBuilder.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();
            modelBuilder.Entity<User>().Property(b => b.IsActive).HasDefaultValueSql("1");
            modelBuilder.Entity<User>().Property(b => b.IsValidAccount).HasDefaultValueSql("1");
            modelBuilder.Entity<User>().Property(b => b.InsertTime).HasDefaultValueSql("CONVERT(DATETIME, CONVERT(VARCHAR(20),GetDate(), 120))");

            modelBuilder.Entity<UserRole>()
                .HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(r => r.RoleId);

            modelBuilder.Entity<UserRole>()
               .HasOne(userRole => userRole.User)
               .WithMany(role => role.Roles)
               .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<UserClaim>()
                .HasOne(userClaim => userClaim.User)
                .WithMany(claim => claim.Claims)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<RoleClaim>()
                 .HasOne(roleclaim => roleclaim.Role)
                 .WithMany(claim => claim.Claims)
                 .HasForeignKey(c => c.RoleId);
        }
    }
}
