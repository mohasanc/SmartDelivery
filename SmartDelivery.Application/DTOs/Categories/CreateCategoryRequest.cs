namespace SmartDelivery.Application.DTOs.Categories
{
    public record CreateCategoryRequest(
        string Name,
        string? Description,
        string? ImageUrl,
        int DisplayOrder,
        Guid RestaurantId
    );
}
