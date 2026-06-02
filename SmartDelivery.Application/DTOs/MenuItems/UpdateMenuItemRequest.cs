namespace SmartDelivery.Application.DTOs.MenuItems
{
    public record UpdateMenuItemRequest(
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
        Guid CategoryId
    );
}
