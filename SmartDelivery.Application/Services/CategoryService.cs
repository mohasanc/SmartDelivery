using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Categories;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Application.Interfaces.Services;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetWithMenuItemsAsync(id, cancellationToken);
            if (category is null || category.IsDeleted)
                return Result<CategoryDto>.NotFound("Category not found.");

            return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(category));
        }
        public async Task<Result<IEnumerable<CategoryDto>>> GetByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result<IEnumerable<CategoryDto>>.NotFound("Restaurant not found");

            var categories = await _unitOfWork.Categories.GetByRestaurantAsync(restaurantId, cancellationToken);

            var dtos = _mapper.Map<IEnumerable<CategoryDto>>(categories.Where(c => !c.IsDeleted).OrderBy(c => c.DisplayOrder));

            return Result<IEnumerable<CategoryDto>>.Success(dtos);

        }

        public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryRequest request, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result<CategoryDto>.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result<CategoryDto>.Forbidden("You are not authorized to modify this restaurant.");

            var category = _mapper.Map<Category>(request);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var created = await _unitOfWork.Categories.GetWithMenuItemsAsync(category.Id, cancellationToken);

            return Result<CategoryDto>.Created(_mapper.Map<CategoryDto>(created!));

        }
        public async Task<Result<CategoryDto>> UpdateASync(Guid id, UpdateCategoryRequest request, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetWithMenuItemsAsync(id, cancellationToken);
            if (category is null || category.IsDeleted)
                return Result<CategoryDto>.NotFound("Category not found.");

            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(category.RestaurantId, cancellationToken);

            if (restaurant is null)
                return Result<CategoryDto>.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result<CategoryDto>.Forbidden("Unauthorized.");

            _mapper.Map(request, category);
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<CategoryDto>(category);

            return Result<CategoryDto>.Success(dto);
        }
        public async Task<Result> DeleteAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category is null || category.IsDeleted)
                return Result.NotFound("Category not found.");

            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(category.RestaurantId, cancellationToken);
            if (restaurant is null || restaurant.IsDeleted)
                return Result.NotFound("Restaurant not found.");

            if (restaurant.OwnerId != requesterId)
                return Result.Failure("Unauthorized.", 403);

            _unitOfWork.Categories.SoftDelete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Category Deleted");
        }
    }
}
