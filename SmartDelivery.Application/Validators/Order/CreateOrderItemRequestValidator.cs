using FluentValidation;
using SmartDelivery.Application.DTOs.Orders;

namespace SmartDelivery.Application.Validators.Order
{
    public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
    {
        public CreateOrderItemRequestValidator()
        {
            RuleFor(x => x.MenuItemId).NotEmpty().WithMessage("Menu item is required.");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.")
                .LessThanOrEqualTo(20).WithMessage("Maximum 20 items per menu item.");
        }
    }
}
