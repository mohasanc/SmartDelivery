using FluentValidation;
using SmartDelivery.Application.DTOs.Orders;
using SmartDelivery.Domain.Enums;

namespace SmartDelivery.Application.Validators.Order
{
    public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
    {
        private static readonly string[] ValidStatuses = Enum.GetNames(typeof(OrderStatus));

        public UpdateOrderStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => ValidStatuses.Contains(s))
                .WithMessage($"Invalid status. Valid values: {string.Join(", ", ValidStatuses)}");
        }
    }
}
