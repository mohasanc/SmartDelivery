using Microsoft.EntityFrameworkCore;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Infrastructure.Data;

namespace SmartDelivery.Infrastructure.Repositories
{
    public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(AppDbContext context) : base(context) { }
        public async Task<Restaurant?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbset
                .Include(r => r.Owner)
                .Include(r => r.Categories.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        public async Task<PagedResult<Restaurant>> GetPagedAsync(RestaurantQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbset.Include(r => r.Owner).AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var term = parameters.SearchTerm.ToLower();
                query = query.Where(r =>
                r.Name.ToLower().Contains(term) ||
                r.City.ToLower().Contains(term) ||
                r.Description.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(parameters.City))
                query = query.Where(r => r.City.ToLower() == parameters.City.ToLower());

            if (parameters.IsOpen.HasValue)
                query = query.Where(r => r.IsOpen == parameters.IsOpen.Value);

            if (parameters.MaxDeliveryFee.HasValue)
                query = query.Where(r => r.DeliveryFee <= parameters.MaxDeliveryFee.Value);

            if (parameters.MinRating.HasValue)
                query = query.Where(r => r.Rating >= parameters.MinRating.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            query = parameters.SortBy?.ToLower() switch
            {
                "rating" => parameters.SortDescending ? query.OrderByDescending(r => r.Rating) : query.OrderBy(r => r.Rating),
                "deliveryfee" => parameters.SortDescending ? query.OrderByDescending(r => r.DeliveryFee) : query.OrderBy(r => r.DeliveryFee),
                "name" => parameters.SortDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
                _ => query.OrderByDescending(r => r.Rating)
            };

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return PagedResult<Restaurant>.Create(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<IEnumerable<Restaurant>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
            => await _dbset.Where(r => r.OwnerId == ownerId).ToListAsync(cancellationToken);
    }
}