using SmartDelivery.Domain.Enums;

namespace SmartDelivery.Application.DTOs.Orders
{
    public record UpdateOrderStatusRequest(
        string Status,
        string? CancellationReason
    );
}
