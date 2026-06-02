using Microsoft.EntityFrameworkCore;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Infrastructure.Data;

namespace SmartDelivery.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default)
            => await _dbset
            .Include(c => c.MenuItems.Where(m => !m.IsDeleted && m.IsActive))
            .Where(c => c.RestaurantId == restaurantId)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);

        public async Task<Category?> GetWithMenuItemsAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbset
                .Include(c => c.MenuItems.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
