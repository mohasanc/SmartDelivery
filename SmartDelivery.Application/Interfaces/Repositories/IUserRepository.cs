using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<User?> GetWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetDriversAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    }
}
