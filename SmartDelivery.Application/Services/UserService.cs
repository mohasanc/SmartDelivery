using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Users;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Application.Interfaces.Services;

namespace SmartDelivery.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetWithRolesAsync(id, cancellationToken);
            if (user == null)
                return Result<UserDto>.Failure("User not found.");

            var dto = _mapper.Map<UserDto>(user);
            var roles = await _unitOfWork.Users.GetUserRolesAsync(id, cancellationToken);
            dto = dto with { Roles = roles };

            return Result<UserDto>.Success(dto);
        }
        public async Task<Result<PagedResult<UserSummaryDto>>> GetAllAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.Users.GetQueryable()
                .Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var term = parameters.SearchTerm.ToLowerInvariant();
                query = query.Where(u =>
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term) ||
                u.PhoneNumber.Contains(term));
            }

            var totalCount = query.Count();

            query = parameters.SortBy?.ToLower() switch
            {
                "name" => parameters.SortDescending ? query.OrderByDescending(u => u.FirstName) :
                query.OrderBy(u => u.FirstName),
                "email" => parameters.SortDescending ? query.OrderByDescending(u => u.Email) :
                query.OrderBy(u => u.Email),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            var users = query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToList();

            var dtos = new List<UserSummaryDto>();
            foreach (var user in users)
            {
                var roles = await _unitOfWork.Users.GetUserRolesAsync(user.Id, cancellationToken);
                var dto = _mapper.Map<UserSummaryDto>(user) with { Roles = roles };
                dtos.Add(dto);
            }

            return Result<PagedResult<UserSummaryDto>>.Success(
                PagedResult<UserSummaryDto>.Create(dtos, totalCount, parameters.PageNumber, parameters.PageSize)
            );

        }
        public async Task<Result<UserDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetWithRolesAsync(userId, cancellationToken);
            if (user is null)
                return Result<UserDto>.Failure("User not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.Latitude = request.Latitude;
            user.Longitude = request.Longitude;
            if (user.ProfileImageUrl is not null)
                user.ProfileImageUrl = request.ProfileImageUrl;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var roles = await _unitOfWork.Users.GetUserRolesAsync(userId, cancellationToken);
            var dto = _mapper.Map<UserDto>(user) with { Roles = roles };

            return Result<UserDto>.Success(dto, "Profile updated successfully.");
        }
        public async Task<Result> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                return Result.NotFound("User not found.");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("User deactivated.");
        }
        public async Task<Result<IEnumerable<UserSummaryDto>>> GetDriversAsync(CancellationToken cancellationToken = default)
        {
            var drivers = await _unitOfWork.Users.GetDriversAsync(cancellationToken);
            var dtos = new List<UserSummaryDto>();

            foreach (var driver in drivers)
            {
                var roles = await _unitOfWork.Users.GetUserRolesAsync(driver.Id, cancellationToken);
                dtos.Add(_mapper.Map<UserSummaryDto>(driver) with { Roles = roles });
            }

            return Result<IEnumerable<UserSummaryDto>>.Success(dtos);
        }
    }
}
