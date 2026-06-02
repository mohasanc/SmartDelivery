using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Users;
using SmartDelivery.Application.Interfaces.Services;

namespace SmartDelivery.API.Controllers
{
    [Tags("Users")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var result = await _userService.GetByIdAsync(CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        [HttpPut("me")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateProfileAsync(CurrentUserId, request, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Get a user by ID. (Admin only)
        /// </summary>
        [HttpGet("{id:Guid}")]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _userService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Get paginated list of all users. (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UserSummaryDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters parameters, CancellationToken cancellationToken)
        {
            var result = await _userService.GetAllAsync(parameters, cancellationToken);
            return HandleResult(result);
        }
        [HttpGet("drivers")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserSummaryDto>>), 200)]
        public async Task<IActionResult> GetDrivers(CancellationToken cancellationToken)
        {
            var result = await _userService.GetDriversAsync(cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await _userService.DeactivateUserAsync(id, cancellationToken);
            return HandleResult(result);
        }
    }
}
