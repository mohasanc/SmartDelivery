using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Restaurants;

namespace SmartDelivery.Application.Interfaces.Services
{
    public interface IRestaurantService
    {
        Task<Result<RestaurantDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<RestaurantSummaryDto>>> GetAllAsync(RestaurantQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<Result<RestaurantDto>> CreateAsync(Guid ownerId, CreateRestaurantRequest requst, CancellationToken cancellationToken = default);
        Task<Result<RestaurantDto>> UpdateAsync(Guid id, Guid requesterId, UpdateRestaurantRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<RestaurantSummaryDto>>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
        Task<Result> ToggleStatusAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default);
    }
}
