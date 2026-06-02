using FluentValidation;
using SmartDelivery.Application.DTOs.Orders;

namespace SmartDelivery.Application.Validators.Order
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.RestaurantId)
                .NotEmpty().WithMessage("Restaurant is required.");

            RuleFor(x => x.DeliveryAddress)
                .NotEmpty().WithMessage("Delivery address is required.")
                .MaximumLength(300).WithMessage("Address too long.");

            RuleFor(x => x.DeliveryLatitude)
                .InclusiveBetween(-90, 90).WithMessage("Invalid latitude.");

            RuleFor(x => x.DeliveryLongitude)
                .InclusiveBetween(-180, 180).WithMessage("Invalid longitude.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .Must(m => new[] { "Cash", "Card", "Wallet" }.Contains(m))
                .WithMessage("Payment method must be Cash, Card, or Wallet.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item.");

            RuleForEach(x => x.Items).SetValidator(new CreateOrderItemRequestValidator());
        }
    }
}
