using Microsoft.EntityFrameworkCore;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Infrastructure.Data;

namespace SmartDelivery.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context) { }
        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
            => await _dbset.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
