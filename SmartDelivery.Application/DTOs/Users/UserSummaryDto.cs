namespace SmartDelivery.Application.DTOs.Users
{
    public record UserSummaryDto
    (
        Guid Id,
        string FullName,
        string Email,
        string PhoneNumber,
        bool IsActive,
        IEnumerable<string> Roles
    );
}
