namespace SmartDelivery.Application.DTOs.MenuItems
{
    public record MenuItemDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string? ImageUrl,
        bool IsAvailable,
        int PreparationTimeMinutes,
        int Calories,
        bool IsVegetarian,
        bool IsVegan,
        bool IsGlutenFree,
        Guid CategoryId,
        string CategoryName,
        Guid RestaurantId
    );
}
