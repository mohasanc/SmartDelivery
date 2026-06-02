using SmartDelivery.Domain.Common;

namespace SmartDelivery.Domain.Entities
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public decimal DeliveryFee { get; set; }
        public int EstimatedDeliveryMinutes { get; set; }
        public double Rating { get; set; }
        public int TotalRatings { get; set; }
        public bool IsOpen { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string OpeningTime { get; set; } = "08:00";
        public string ClosingTime { get; set; } = "22:00";
        public Guid OwnerId { get; set; }

        public User Owner { get; set; } = null!;
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
