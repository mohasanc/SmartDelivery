using SmartDelivery.Application.Common;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<Order>> GetPagedAsync(OrderQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByDriverAsync(Guid driverId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default);
        Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);
    }
}
