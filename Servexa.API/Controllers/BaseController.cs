using Microsoft.AspNetCore.Mvc;
using Servexa.Shared.Responses;

namespace Servexa.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "")
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        protected IActionResult SuccessMessage(string message)
        {
            return Ok(ApiResponse<string>.SuccessResponse(default, message));
        }

        protected IActionResult Error(string message)
        {
            return BadRequest(ApiResponse<string>.Error(message));
        }
    }
}
