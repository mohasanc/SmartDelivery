namespace SmartDelivery.Application.DTOs.Restaurants
{
    public record CreateRestaurantRequest(
        string Name,
        string Description,
        string PhoneNumber,
        string Email,
        string Address,
        string City,
        double Latitude,
        double Longitude,
        string? LogoUrl,
        string? CoverImageUrl,
        decimal DeliveryFee,
        int EstimatedDeliveryMinutes,
        string OpeningTime,
        string ClosingTime
    );
}
