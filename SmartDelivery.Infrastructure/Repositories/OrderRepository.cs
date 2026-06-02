using Microsoft.EntityFrameworkCore;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Domain.Enums;
using SmartDelivery.Infrastructure.Data;

namespace SmartDelivery.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }
        public async Task<Order?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbset
                .Include(o => o.Customer)
                .Include(o => o.Restaurant)
                .Include(o => o.Driver)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        public async Task<PagedResult<Order>> GetPagedAsync(OrderQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbset
                .Include(o => o.Customer)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (parameters.CustomerId.HasValue)
                query = query.Where(o => o.CustomerId == parameters.CustomerId.Value);

            if (parameters.RestaurantId.HasValue)
                query = query.Where(o => o.RestaurantId == parameters.RestaurantId.Value);

            if (parameters.DriverId.HasValue)
                query = query.Where(o => o.DriverId == parameters.DriverId.Value);

            if (!string.IsNullOrWhiteSpace(parameters.Status)
                && Enum.TryParse<OrderStatus>(parameters.Status, true, out var status))
                query = query.Where(o => o.Status == status);

            if (parameters.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= parameters.FromDate.Value);

            if (parameters.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= parameters.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
                query = query.Where(o => o.OrderNumber.Contains(parameters.SearchTerm));

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return PagedResult<Order>.Create(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
            => await _dbset
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Order>> GetByDriverAsync(Guid driverId, CancellationToken cancellationToken = default)
             => await _dbset
            .Include(o => o.Restaurant)
            .Include(o => o.Customer)
            .Where(o => o.DriverId == driverId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Order>> GetByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default)
            => await _dbset
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .Where(o => o.RestaurantId == restaurantId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
        public async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var count = await _dbset.CountAsync(o => o.CreatedAt.Date == DateTime.UtcNow.Date, cancellationToken);
            return $"SD-{today}-{(count + 1):D4}";
        }
        
    }
}
