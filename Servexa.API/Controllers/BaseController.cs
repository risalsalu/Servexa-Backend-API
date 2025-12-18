using Microsoft.AspNetCore.Mvc;
using Servexa.Shared.Responses;

namespace Servexa.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "")
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, 200, message));
        }

        protected IActionResult Created<T>(T data, string message = "")
        {
            return StatusCode(201, ApiResponse<T>.SuccessResponse(data, 201, message));
        }

        protected IActionResult SuccessMessage(string message)
        {
            return Ok(ApiResponse<string>.SuccessResponse(message, 200, message));
        }

        protected IActionResult BadRequestError(string message)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(message, 400));
        }

        protected IActionResult NotFoundError(string message)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(message, 404));
        }

        protected IActionResult UnauthorizedError(string message)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse(message, 401));
        }

        protected IActionResult ForbiddenError(string message)
        {
            return StatusCode(403, ApiResponse<object>.ErrorResponse(message, 403));
        }

        protected IActionResult ServerError(string message)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse(message, 500));
        }
    }
}
