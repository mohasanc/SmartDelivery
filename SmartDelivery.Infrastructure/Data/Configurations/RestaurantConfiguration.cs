using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Infrastructure.Data.Configurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.ToTable("Restaurants");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Description).IsRequired().HasMaxLength(500);
            builder.Property(r => r.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(r => r.Email).IsRequired().HasMaxLength(150);
            builder.Property(r => r.Address).IsRequired().HasMaxLength(300);
            builder.Property(r => r.City).IsRequired().HasMaxLength(100);
            builder.Property(r => r.LogoUrl).HasMaxLength(500);
            builder.Property(r => r.CoverImageUrl).HasMaxLength(500);
            builder.Property(r => r.DeliveryFee).HasColumnType("decimal(18,2)");
            builder.Property(r => r.Rating).HasColumnType("float");
            builder.Property(r => r.OpeningTime).HasMaxLength(5);
            builder.Property(r => r.ClosingTime).HasMaxLength(5);
        }
    }
}
