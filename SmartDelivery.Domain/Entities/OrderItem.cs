using SmartDelivery.Domain.Common;

namespace SmartDelivery.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty; 
        public decimal UnitPrice { get; set; }                   
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SpecialRequests { get; set; }

        public Order Order { get; set; } = null!;
        public MenuItem MenuItem { get; set; } = null!;
    }
}
