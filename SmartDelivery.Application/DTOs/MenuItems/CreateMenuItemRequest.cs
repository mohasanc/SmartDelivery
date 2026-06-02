namespace SmartDelivery.Application.DTOs.MenuItems
{
    public record CreateMenuItemRequest(
        string Name,
        string Description,
        decimal Price,
        string? ImageUrl,
        int PreparationTimeMinutes,
        int Calories,
        bool IsVegetarian,
        bool IsVegan,
        bool IsGlutenFree,
        Guid CategoryId,
        Guid RestaurantId
    );
}
