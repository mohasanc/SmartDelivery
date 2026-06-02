namespace SmartDelivery.Application.DTOs.Auth
{
    public record RegisterRequest (
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string Password,
        string ConfirmPassword,
        string Role = "Customer"
    );
}
