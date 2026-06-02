using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Interfaces.Repositories
{
    public interface ICategoryRepository  : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default);
        Task<Category?> GetWithMenuItemsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
