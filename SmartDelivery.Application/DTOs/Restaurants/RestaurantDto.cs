namespace SmartDelivery.Application.DTOs.Restaurants
{
    public record RestaurantDto(
        Guid Id,
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
        double Rating,
        int TotalRatings,
        bool IsOpen,
        bool IsActive,
        string OpeningTime,
        string ClosingTime,
        Guid OwnerId,
        string OwnerName,
        DateTime CreatedAt
    );
}
