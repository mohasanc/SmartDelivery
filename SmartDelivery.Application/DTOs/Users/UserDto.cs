namespace SmartDelivery.Application.DTOs.Users
{
    public record UserDto
    (
        Guid Id,
        string FirstName,
        string LastName,
        string FullName,
        string Email,
        string PhoneNumber,
        string? ProfileImageUrl,
        string? Address,
        bool IsActive,
        bool IsEmailVerified,
        IEnumerable<string> Roles,
        DateTime CreatedAt
    );
}
