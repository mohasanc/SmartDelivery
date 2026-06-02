using Microsoft.EntityFrameworkCore;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Infrastructure.Data;

namespace SmartDelivery.Infrastructure.Repositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(AppDbContext context) : base(context) { }
        public async Task<PagedResult<MenuItem>> GetPagedAsync(Guid restaurantId, MenuItemQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbset
                .Include(m => m.Category)
                .Where(m => m.RestaurantId == restaurantId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var term = parameters.SearchTerm.ToLower();
                query = query.Where(m => m.Name.ToLower().Contains(term) || m.Description.ToLower().Contains(term));
            }

            if (parameters.CategoryId.HasValue)
                query = query.Where(m => m.CategoryId == parameters.CategoryId.Value);

            if (parameters.IsAvailable.HasValue)
                query = query.Where(m => m.IsAvailable == parameters.IsAvailable.Value);

            if (parameters.IsVegetarian.HasValue)
                query = query.Where(m => m.IsVegetarian == parameters.IsVegetarian.Value);

            if (parameters.IsVegan.HasValue)
                query = query.Where(m => m.IsVegan == parameters.IsVegan.Value);

            if (parameters.IsGlutenFree.HasValue)
                query = query.Where(m => m.IsGlutenFree == parameters.IsGlutenFree.Value);

            if (parameters.MinPrice.HasValue)
                query = query.Where(m => m.Price >= parameters.MinPrice.Value);

            if (parameters.MaxPrice.HasValue)
                query = query.Where(m => m.Price <= parameters.MaxPrice.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            query = parameters.SortBy?.ToLower() switch
            {
                "price" => parameters.SortDescending ? query.OrderByDescending(m => m.Price) : query.OrderBy(m => m.Price),
                "name" => parameters.SortDescending ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Price),
                _ => query.OrderBy(m => m.Name)
            };

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return PagedResult<MenuItem>.Create(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }
        public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
            => await _dbset.Where(m => m.CategoryId == categoryId).ToListAsync(cancellationToken);

        public async Task<IEnumerable<MenuItem>> GetByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default)
            => await _dbset
                .Include(m => m.Category)
                .Where(m => m.RestaurantId == restaurantId)
                .ToListAsync(cancellationToken);
    }
}
