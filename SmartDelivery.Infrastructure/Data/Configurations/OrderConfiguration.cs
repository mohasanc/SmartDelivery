using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(20);
            builder.Property(o => o.DeliveryAddress).IsRequired().HasMaxLength(300);
            builder.Property(o => o.SpecialInstructions).HasMaxLength(500);
            builder.Property(o => o.PaymentMethod).IsRequired().HasMaxLength(20);
            builder.Property(o => o.CancellationReason).HasMaxLength(500);
            builder.Property(o => o.CustomerReview).HasMaxLength(500);
            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(o => o.DeliveryFee).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Tax).HasColumnType("decimal(18,2)");
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Status).HasConversion<int>();

            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.HasOne(o => o.Restaurant)
           .WithMany(r => r.Orders)
           .HasForeignKey(o => o.RestaurantId)
           .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
