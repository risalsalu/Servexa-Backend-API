using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.API.Controllers;
using Servexa.Application.DTOs.Auth.Common;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        private Guid GetUserId()
            => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = GetUserId();
            var profile = await _authService.GetCurrentUserAsync(userId);
            return Success(profile, "Profile fetched");
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            var userId = GetUserId();
            await _authService.UpdateProfileAsync(userId, dto);
            return SuccessMessage("Profile updated");
        }
    }
}
