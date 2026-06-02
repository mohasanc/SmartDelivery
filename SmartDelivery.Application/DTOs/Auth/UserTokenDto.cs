namespace SmartDelivery.Application.DTOs.Auth
{
    public record UserTokenDto
    (
        Guid Id,
        string FullName,
        string Email,
        string PhoneNumber,
        IEnumerable<string> Roles
    );
}
