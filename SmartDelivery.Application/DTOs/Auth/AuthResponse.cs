namespace SmartDelivery.Application.DTOs.Auth
{
    public record AuthResponse
    (
        string RefreshToken,
        string AccessToken,
        DateTime ExpiresAt,
        UserTokenDto User
    );
}
