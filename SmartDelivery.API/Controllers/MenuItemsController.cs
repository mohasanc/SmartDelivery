using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.MenuItems;
using SmartDelivery.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace SmartDelivery.API.Controllers
{
    /// <summary>
    /// Manage menu items within a restaurant.
    /// </summary>

    [Tags("Menu Items")]
    public class MenuItemsController : BaseController
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemsController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        /// <summary>
        /// Get paginated, filtered menu items for a restaurant.
        /// </summary>
        [HttpGet("restaurant/{restaurantId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MenuItemDto>>), 200)]
        public async Task<IActionResult> GetByRestaurant(Guid restaurantId, [FromQuery] MenuItemQueryParameters parameters, CancellationToken cancellationToken)
        {
            var result = await _menuItemService.GetByRestaurantAsync(restaurantId, parameters, cancellationToken);
            return HandleResult(result);
        }
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<MenuItemDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _menuItemService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles =("RestaurantOwner,Admin"))]
        [ProducesResponseType(typeof(ApiResponse<MenuItemDto>), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemRequest request, CancellationToken cancellationToken)
        {
            var result = await _menuItemService.CreateAsync(request, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = ("RestaurantOwner,Admin"))]
        [ProducesResponseType(typeof(ApiResponse<MenuItemDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateMenuItemRequest request, CancellationToken cancellationToken)
        {
            var result = await _menuItemService.UpdateAsync(id, request, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = ("RestaurantOwner,Admin"))]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _menuItemService.DeleteAsync(id, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        [HttpPatch("{id:guid}/toggle-availability")]
        [Authorize(Roles = ("RestaurantOwner,Admin"))]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> ToggleAvailability(Guid id, CancellationToken cancellationToken)
        {
            var result = await _menuItemService.ToggleAvailabilityAsync(id, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }
    }
}
