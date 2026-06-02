namespace SmartDelivery.Application.DTOs.Users
{
    public record UpdateProfileRequest
    (
        string FirstName,
        string LastName,
        string PhoneNumber,
        string? Address,
        double? Latitude,
        double? Longitude,
        string? ProfileImageUrl
    );
}
