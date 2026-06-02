using SmartDelivery.Application.Common;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Interfaces.Repositories
{
    public interface IRestaurantRepository : IGenericRepository<Restaurant>
    {
        Task<Restaurant?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<Restaurant>> GetPagedAsync(RestaurantQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<Restaurant>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
    }
}
