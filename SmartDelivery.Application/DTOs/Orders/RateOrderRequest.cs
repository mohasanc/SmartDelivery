namespace SmartDelivery.Application.DTOs.Orders
{
    public record RateOrderRequest(
        int Rating,
        string? Review
    );
}
