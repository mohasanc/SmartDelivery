using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Categories;

namespace SmartDelivery.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<CategoryDto>>> GetByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default);
        Task<Result<CategoryDto>> CreateAsync(CreateCategoryRequest request, Guid requesterId, CancellationToken cancellationToken = default);
        Task<Result<CategoryDto>> UpdateASync(Guid id, UpdateCategoryRequest request, Guid requesterId, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, Guid requesterId, CancellationToken cancellationToken = default);
    }
}
