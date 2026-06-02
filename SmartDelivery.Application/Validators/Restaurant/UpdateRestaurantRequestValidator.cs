using FluentValidation;
using SmartDelivery.Application.DTOs.Restaurants;

namespace SmartDelivery.Application.Validators.Restaurant
{
    public class UpdateRestaurantRequestValidator :AbstractValidator<UpdateRestaurantRequest>
    {
        public UpdateRestaurantRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.DeliveryFee).GreaterThanOrEqualTo(0);
            RuleFor(x => x.EstimatedDeliveryMinutes).GreaterThan(0).LessThanOrEqualTo(180);
        }
    }
}
