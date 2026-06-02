namespace SmartDelivery.Application.DTOs.Orders
{
    public record CreateOrderRequest(
        Guid RestaurantId,
        string DeliveryAddress,
        double DeliveryLatitude,
        double DeliveryLongitude,
        string? SpecialInstructions,
        string PaymentMethod,
        IEnumerable<CreateOrderItemRequest> Items
    );
}
