using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Users;

namespace SmartDelivery.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Result<PagedResult<UserSummaryDto>>> GetAllAsync(QueryParameters parameters, CancellationToken cancellationToken = default);
        Task<Result<UserDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<UserSummaryDto>>> GetDriversAsync(CancellationToken cancellationToken = default);
    }
}
