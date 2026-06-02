using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartDelivery.Application.Common;

namespace SmartDelivery.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        protected Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User not found."));

        protected IEnumerable<string> CurrentUserRoles =>
            User.FindAll(ClaimTypes.Role).Select(c => c.Value);

        protected bool IsInRole(string role) => User.IsInRole(role);

        /// <summary>
        /// Converts a Result&lt;T&gt; to the appropriate HTTP response.
        /// </summary>
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return result.StatusCode switch
                {
                    201 => Created(string.Empty, new ApiResponse<T>(result.StatusCode, result.Message, result.Data)),
                    204 => NoContent(),
                    _ => Ok(new ApiResponse<T>(result.StatusCode, result.Message, result.Data))
                };
            }
            return result.StatusCode switch
            {
                401 => Unauthorized(new ApiErrorResponse(result.StatusCode, "Unauthorized", result.Errors)),
                403 => StatusCode(403, new ApiErrorResponse(result.StatusCode, "Forbidden", result.Errors)),
                404 => NotFound(new ApiErrorResponse(result.StatusCode, "Not Found", result.Errors)),
                _ => BadRequest(new ApiErrorResponse(result.StatusCode, "Bad Request", result.Errors))
            };
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok(new ApiResponse<object>(result.StatusCode, result.Message, null));

            return result.StatusCode switch
            {
                401 => Unauthorized(new ApiErrorResponse(result.StatusCode, "Unauthorized", result.Errors)),
                403 => StatusCode(403, new ApiErrorResponse(result.StatusCode, "Forbidden", result.Errors)),
                404 => NotFound(new ApiErrorResponse(result.StatusCode, "Not Found", result.Errors)),
                _ => BadRequest(new ApiErrorResponse(result.StatusCode, "Bad Request", result.Errors))
            };
        }
    }
    public record ApiResponse<T>(int StatusCode, string? Message, T? Data);
    public record ApiErrorResponse(int StatusCode, string Message, IEnumerable<string> Errors);
}
