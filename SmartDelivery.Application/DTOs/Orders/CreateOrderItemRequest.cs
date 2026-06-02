namespace SmartDelivery.Application.DTOs.Orders
{
    public record CreateOrderItemRequest(
        Guid MenuItemId,
        int Quantity,
        string? SpecialRequests
    );
}
