namespace SmartDelivery.Application.DTOs.Users
{
    public record DriverLocationDto
    (
        Guid Id,
        string DriverName,
        double Latitude,
        double Longitude,
        DateTime LastUpdated
    );
}
