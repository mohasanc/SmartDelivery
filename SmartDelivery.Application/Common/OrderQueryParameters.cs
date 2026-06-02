namespace SmartDelivery.Application.Common
{
    public class OrderQueryParameters : QueryParameters
    {
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? RestaurantId { get; set; }
        public Guid? DriverId { get; set; }
    }
}
