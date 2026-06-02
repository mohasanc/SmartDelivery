namespace SmartDelivery.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
        string GenerateRefreshToken();
        (Guid userId, string email, IEnumerable<string> roles) ValidateToken(string token);
        bool IsTokenExpired(string token);
    }
}
