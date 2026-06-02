using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Orders;
using SmartDelivery.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace SmartDelivery.API.Controllers
{
    [Tags("Orders")]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderSummaryDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] OrderQueryParameters parameters, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetAllAsync(parameters, cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("my-orders")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderSummaryDto>>), 200)]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderQueryParameters parameters, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetMyOrdersAsync(CurrentUserId, parameters, cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("driver-orders")]
        [Authorize(Roles = "Driver")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderSummaryDto>>), 200)]
        public async Task<IActionResult> GetDriverOrders([FromQuery] OrderQueryParameters parameters, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetDriverOrdersAsync(CurrentUserId, parameters, cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetByIdAsync(id, CurrentUserId, CurrentUserRoles, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderDto>>), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.CreateAsync(CurrentUserId, request, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Update the status of an order.
        /// Allowed transitions: Pending → Confirmed → Preparing → ReadyForPickup → OnTheWay → Delivered.
        /// Cancellation is available from Pending or Confirmed.
        /// </summary>

        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.UpdateStatusAsync(id, request, CurrentUserId, CurrentUserRoles, cancellationToken);
            return HandleResult(result);
        }

        [HttpPatch("{id:guid}/assign-driver")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> AssignDriver(Guid id, AssignDriverRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.AssignDriverAsync(id, request, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost("{id:guid}/rate")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> RateOrder(Guid id, RateOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.RateOrderAsync(id, CurrentUserId, request, cancellationToken);
            return HandleResult(result);
        }
    }
}
