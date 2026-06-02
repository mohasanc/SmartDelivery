namespace SmartDelivery.Application.DTOs.Restaurants
{
    public record RestaurantSummaryDto(
        Guid Id,
        string Name,
        string? LogoUrl,
        string City,
        decimal DeliveryFee,
        int EstimatedDeliveryMinutes,
        double Rating,
        bool IsOpen
    );
}
