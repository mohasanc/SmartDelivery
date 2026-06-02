using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Restaurants;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Application.Interfaces.Services;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger<RestaurantService> _logger;

        private const string CacheKeyPrefix = "restaurant:";

        public RestaurantService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, ILogger<RestaurantService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _logger = logger;
        }
        public async Task<Result<RestaurantDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";
            var cached = await _cacheService.GetAsync<RestaurantDto>(cacheKey, cancellationToken);
            if (cached is not null)
                return Result<RestaurantDto>.Success(cached);

            var restaurant = await _unitOfWork.Restaurants.GetWithDetailsAsync(id, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result<RestaurantDto>.NotFound("Restaurant not found.");

            var dto = _mapper.Map<RestaurantDto>(restaurant);
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), cancellationToken);

            return Result<RestaurantDto>.Success(dto);
        }
        public async Task<Result<PagedResult<RestaurantSummaryDto>>> GetAllAsync(RestaurantQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.Restaurants.GetPagedAsync(parameters, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<RestaurantSummaryDto>>(result.Items);

            return Result<PagedResult<RestaurantSummaryDto>>.Success(PagedResult<RestaurantSummaryDto>.
                Create(dtos, result.TotalCount, result.PageNumber, result.PageSize));
        }
        public async Task<Result<RestaurantDto>> CreateAsync(Guid ownerId, CreateRestaurantRequest requst, CancellationToken cancellationToken = default)
        {
            var owner = await _unitOfWork.Users.GetByIdAsync(ownerId, cancellationToken);
            if (owner is null)
                return Result<RestaurantDto>.NotFound("Owner not found."); ;

            var restaurant = _mapper.Map<Restaurant>(requst);
            restaurant.OwnerId = ownerId;

            await _unitOfWork.Restaurants.AddAsync(restaurant, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Restaurant '{Name}' created by owner {OwnerId}", restaurant.Name, ownerId);

            var created = await _unitOfWork.Restaurants.GetWithDetailsAsync(ownerId, cancellationToken);
            var dto = _mapper.Map<RestaurantDto>(created!);

            return Result<RestaurantDto>.Created(dto);
        }
        public async Task<Result<RestaurantDto>> UpdateAsync(Guid id, Guid requesterId, UpdateRestaurantRequest request, CancellationToken cancellationToken = default)
        {
            var restaurant = await _unitOfWork.Restaurants.GetWithDetailsAsync(id, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result<RestaurantDto>.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result<RestaurantDto>.Forbidden("You are not the owner of this restaurant.");

            _mapper.Map(request, restaurant);
            restaurant.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Restaurants.Update(restaurant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync($"{CacheKeyPrefix}{id}", cancellationToken);

            var dto = _mapper.Map<RestaurantDto>(restaurant);
            return Result<RestaurantDto>.Success(dto, "Restaurant updated.");
        }
        public async Task<Result> DeleteAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(id, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result.Failure("You are not the owner of this restaurant.", 403);

            _unitOfWork.Restaurants.SoftDelete(restaurant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync($"{CacheKeyPrefix}{id}", cancellationToken);
            return Result.Success("Restaurant deleted.");
        }

        public async Task<Result<IEnumerable<RestaurantSummaryDto>>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
        {
            var restaurants = await _unitOfWork.Restaurants.GetByOwnerAsync(ownerId, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<RestaurantSummaryDto>>(restaurants);
            return Result<IEnumerable<RestaurantSummaryDto>>.Success(dtos);
        }

        public async Task<Result> ToggleStatusAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(id, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result.Failure("Unauthorized.", 403);

            restaurant.IsOpen = !restaurant.IsOpen;
            restaurant.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Restaurants.Update(restaurant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync($"{CacheKeyPrefix}{id}", cancellationToken);
            return Result.Success($"Restaurant is now {(restaurant.IsOpen ? "open" : "closed")}.");
        }
    }
}
