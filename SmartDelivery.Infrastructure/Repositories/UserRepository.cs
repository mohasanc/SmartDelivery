using Microsoft.EntityFrameworkCore;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Infrastructure.Data;

namespace SmartDelivery.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => await _dbset.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
            => await _dbset.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

        public async Task<User?> GetWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbset
                .Include(u => u.Roles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        public async Task<IEnumerable<User>> GetDriversAsync(CancellationToken cancellationToken = default)
            => await _dbset
                .Include(u => u.Roles).ThenInclude(ur => ur.Role)
                .Where(u => u.Roles.Any(ur => ur.Role.Name == "Driver") && u.IsActive)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.Name)
                .ToListAsync(cancellationToken);

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
            => await _dbset.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

        public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            var exists = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
            if (!exists)
                await _context.UserRoles.AddAsync(new UserRole { UserId = userId, RoleId = roleId }, cancellationToken);
        }

    }
}
