using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.MenuItems;

namespace SmartDelivery.Application.Interfaces.Services
{
    public interface IMenuItemService
    {
        Task<Result<MenuItemDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<MenuItemDto>>> GetByRestaurantAsync(Guid restaurantId, MenuItemQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<Result<MenuItemDto>> CreateAsync(CreateMenuItemRequest request, Guid requesterId, CancellationToken cancellationToken = default);
        Task<Result<MenuItemDto>> UpdateAsync(Guid id, UpdateMenuItemRequest request, Guid requesterId, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default);
        Task<Result> ToggleAvailabilityAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default);
    }
}
