using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);
            builder.Property(u => u.Address).HasMaxLength(300);
            builder.Property(u => u.RefreshToken).HasMaxLength(500);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasMany(u => u.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.DriverOrders)
           .WithOne(o => o.Driver)
           .HasForeignKey(o => o.DriverId)
           .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.OwnedRestaurants)
           .WithOne(r => r.Owner)
           .HasForeignKey(r => r.OwnerId)
           .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
