namespace SmartDelivery.Application.DTOs.Categories
{
    public record UpdateCategoryRequest(
        string Name,
        string? Description,
        string? ImageUrl,
        int DisplayOrder,
        bool IsActive
    );
}
