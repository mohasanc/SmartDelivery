using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartDelivery.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value?.Errors.Any() == true)
                    .SelectMany(ms => ms.Value!.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                context.Result = new BadRequestObjectResult(new
                {
                    statusCode = 400,
                    message = "Validation failed.",
                    errors
                });

                return;
            }

            await next();
        }
    }

}
