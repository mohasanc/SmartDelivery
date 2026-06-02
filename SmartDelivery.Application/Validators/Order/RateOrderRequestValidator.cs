using FluentValidation;
using SmartDelivery.Application.DTOs.Orders;

namespace SmartDelivery.Application.Validators.Order
{
    public class RateOrderRequestValidator : AbstractValidator<RateOrderRequest>
    {
        public RateOrderRequestValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Review)
                .MaximumLength(500).WithMessage("Review cannot exceed 500 characters.")
                .When(x => x.Review is not null);
        }
    }
}
