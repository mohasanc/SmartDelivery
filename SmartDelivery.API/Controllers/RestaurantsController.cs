using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Restaurants;
using SmartDelivery.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace SmartDelivery.API.Controllers
{
    [Tags("Restaurants")]
    public class RestaurantsController : BaseController
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        /// <summary>
        /// Get paginated list of restaurants with optional filtering and search.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<RestaurantSummaryDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] RestaurantQueryParameters parameters, CancellationToken cancellationToken)
        {
            var result = await _restaurantService.GetAllAsync(parameters, cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<RestaurantDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _restaurantService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("my-restaurants")]
        [Authorize(Roles = ("RestaurantOwner,Admin"))]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RestaurantSummaryDto>>), 200)]
        public async Task<IActionResult> GetMyRestaurants(CancellationToken cancellationToken)
        {
            var result = await _restaurantService.GetByOwnerAsync(CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        [ProducesResponseType(typeof(ApiResponse<RestaurantDto>), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateRestaurantRequest request, CancellationToken cancellationToken)
        {
            var result = await _restaurantService.CreateAsync(CurrentUserId, request, cancellationToken);
            return HandleResult(result);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        [ProducesResponseType(typeof(ApiResponse<RestaurantDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateRestaurantRequest request, CancellationToken cancellationToken)
        {
            var result = await _restaurantService.UpdateAsync(id, CurrentUserId, request, cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _restaurantService.DeleteAsync(id, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Toggle the open/closed status of a restaurant. (Owner or Admin)
        /// </summary>
        [HttpPatch("{id:Guid}/toggle-status")]
        [Authorize(Roles = "RestaurantOwner,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken cancellationToken)
        {
            var result = await _restaurantService.ToggleStatusAsync(id, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }
    }
}
