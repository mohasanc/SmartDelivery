using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Orders;

namespace SmartDelivery.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Result<OrderDto>> GetByIdAsync(Guid id, Guid requesterId, IEnumerable<string> roles, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<OrderSummaryDto>>> GetAllAsync(OrderQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<OrderSummaryDto>>> GetMyOrdersAsync(Guid customerId, OrderQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<OrderSummaryDto>>> GetDriverOrdersAsync(Guid driverId, OrderQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<Result<OrderDto>> CreateAsync(Guid customerId, CreateOrderRequest request, CancellationToken cancellationToken = default);
        Task<Result<OrderDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, Guid requesterId, IEnumerable<string> roles,
            CancellationToken cancellationToken = default);
        Task<Result<OrderDto>> AssignDriverAsync(Guid orderId, AssignDriverRequest request, CancellationToken cancellationToken = default);
        Task<Result> RateOrderAsync(Guid orderId, Guid customerId, RateOrderRequest request, CancellationToken cancellationToken = default);
    }
}
