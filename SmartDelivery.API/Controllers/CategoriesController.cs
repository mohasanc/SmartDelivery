using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDelivery.Application.DTOs.Categories;
using SmartDelivery.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace SmartDelivery.API.Controllers
{
    [Tags("Categories")]
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("restaurant/{restaurantId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), 200)]
        public async Task<IActionResult> GetByRestaurant(Guid restaurantId, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByRestaurantAsync(restaurantId, cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles =("RestaurantOwner,Admin"))]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _categoryService.CreateAsync(request, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = ("RestaurantOwner,Admin"))]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _categoryService.UpdateASync(id, request, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = ("RestaurantOwner,Admin"))]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.DeleteAsync(id, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }
    }
}
