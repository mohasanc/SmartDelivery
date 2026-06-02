using SmartDelivery.Application.Common;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Interfaces.Repositories
{
    public interface IMenuItemRepository : IGenericRepository<MenuItem>
    {
        Task<PagedResult<MenuItem>> GetPagedAsync(Guid restaurantId, MenuItemQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<MenuItem>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MenuItem>> GetByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    }
}
