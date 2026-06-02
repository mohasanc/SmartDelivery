using FluentValidation;
using SmartDelivery.Application.DTOs.Restaurants;

namespace SmartDelivery.Application.Validators.Restaurant
{
    public class CreateRestaurantRequestValidator : AbstractValidator<CreateRestaurantRequest>
    {
        public CreateRestaurantRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Restaurant name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^01[0-2,5]\d{8}$").WithMessage("Invalid phone number.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Invalid latitude.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Invalid longitude.");

            RuleFor(x => x.DeliveryFee)
                .GreaterThanOrEqualTo(0).WithMessage("Delivery fee cannot be negative.");

            RuleFor(x => x.EstimatedDeliveryMinutes)
                .GreaterThan(0).WithMessage("Estimated delivery time must be positive.")
                .LessThanOrEqualTo(180).WithMessage("Estimated delivery cannot exceed 180 minutes.");

            RuleFor(x => x.OpeningTime)
                .NotEmpty().WithMessage("Opening time is required.")
                .Matches(@"^([01]\d|2[0-3]):([0-5]\d)$").WithMessage("Opening time must be in HH:mm format.");

            RuleFor(x => x.ClosingTime)
                .NotEmpty().WithMessage("Closing time is required.")
                .Matches(@"^([01]\d|2[0-3]):([0-5]\d)$").WithMessage("Closing time must be in HH:mm format.");
        }
    }
}
