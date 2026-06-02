namespace SmartDelivery.Application.DTOs.Categories
{
    public record CategoryDto(
        Guid Id,
        string Name,
        string? Description,
        string? ImageUrl,
        int DisplayOrder,
        bool IsActive,
        Guid RestaurantId,
        int MenuItemCount
    );
}
