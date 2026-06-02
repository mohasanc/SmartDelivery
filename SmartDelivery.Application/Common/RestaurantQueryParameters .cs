namespace SmartDelivery.Application.Common
{
    public class RestaurantQueryParameters : QueryParameters
    {
        public string? City { get; set; }
        public bool? IsOpen { get; set; }
        public decimal? MaxDeliveryFee { get; set; }
        public double? MinRating { get; set; }
    }
}
