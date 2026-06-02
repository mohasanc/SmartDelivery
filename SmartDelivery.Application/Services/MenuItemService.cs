using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.MenuItems;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Application.Interfaces.Services;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MenuItemService> _logger;

        public MenuItemService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<MenuItemDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(id, cancellationToken);
            if (item is null || item.IsDeleted)
                return Result<MenuItemDto>.NotFound("Menu item not found.");

            var dto = _mapper.Map<MenuItemDto>(item);
            return Result<MenuItemDto>.Success(dto);
        }

        public async Task<Result<PagedResult<MenuItemDto>>> GetByRestaurantAsync(Guid restaurantId, MenuItemQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result<PagedResult<MenuItemDto>>.NotFound("Restaurant not found.");

            var result = await _unitOfWork.MenuItems.GetPagedAsync(restaurantId, parameters, cancellationToken);

            var dtos = _mapper.Map<IEnumerable<MenuItemDto>>(result.Items);

            return Result<PagedResult<MenuItemDto>>.Success(
                PagedResult<MenuItemDto>.Create(dtos, result.TotalCount, result.PageNumber, result.PageSize));
        }
        public async Task<Result<MenuItemDto>> CreateAsync(CreateMenuItemRequest request, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result<MenuItemDto>.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result<MenuItemDto>.Forbidden("You are not the owner of this restaurant.");

            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category is null || category.IsDeleted || category.RestaurantId != request.RestaurantId)
                return Result<MenuItemDto>.Failure("Category not found or does not belong to this restaurant.");

            var menuItem = _mapper.Map<MenuItem>(request);
            await _unitOfWork.MenuItems.AddAsync(menuItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("MenuItem '{Name}' created for restaurant {RestaurantId}", menuItem.Name, request.RestaurantId);

            var dto = _mapper.Map<MenuItemDto>(menuItem);
            return Result<MenuItemDto>.Created(dto);
        }
        public async Task<Result<MenuItemDto>> UpdateAsync(Guid id, UpdateMenuItemRequest request, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(id, cancellationToken);
            if (item is null || item.IsDeleted)
                return Result<MenuItemDto>.NotFound("Menu item not found.");

            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(item.RestaurantId, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result<MenuItemDto>.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result<MenuItemDto>.Forbidden("Unauthorized.");

            _mapper.Map(request, item);
            item.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.MenuItems.Update(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<MenuItemDto>(item);

            return Result<MenuItemDto>.Success(dto);
        }

        public async Task<Result> DeleteAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(id, cancellationToken);
            if (item is null || item.IsDeleted)
                return Result.NotFound("Menu item not found.");

            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(item.RestaurantId, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result.Failure("Unauthorized.", 403);

            _unitOfWork.MenuItems.SoftDelete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Menu item deleted");
        }

        public async Task<Result> ToggleAvailabilityAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(id, cancellationToken);
            if (item is null || item.IsDeleted)
                return Result.NotFound("Item not found.");

            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(item.RestaurantId, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result.Failure("Unauthorized.", 403);

            item.IsAvailable = !item.IsAvailable;
            item.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.MenuItems.Update(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success($"Menu item is now {(item.IsAvailable ? "available" : "unavailable")}");
        }
    }
}
