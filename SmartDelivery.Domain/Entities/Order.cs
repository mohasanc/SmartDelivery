using SmartDelivery.Domain.Common;
using SmartDelivery.Domain.Enums;

namespace SmartDelivery.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid? DriverId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }
        public string? SpecialInstructions { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public bool IsPaid { get; set; } = false;
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? PreparingAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public string? CancellationReason { get; set; }
        public int? CustomerRating { get; set; }
        public string? CustomerReview { get; set; }


        public User Customer { get; set; } = null!;
        public Restaurant Restaurant { get; set; } = null!;
        public User? Driver { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
