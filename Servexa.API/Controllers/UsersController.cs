using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Users;
using Servexa.Application.Interfaces;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var result = await _authService.GetCurrentUserAsync(GetUserId());
            return Success(result, "User profile fetched successfully");
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            await _authService.UpdateProfileAsync(GetUserId(), dto);
            return SuccessMessage("Profile updated successfully");
        }

        [HttpPatch("me/contact")]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactInfoDto dto)
        {
            await _authService.UpdateContactInfoAsync(GetUserId(), dto);
            return SuccessMessage("Contact information updated successfully");
        }

        [HttpPost("me/profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var url = await _authService.UploadProfileImageAsync(GetUserId(), file);
            return Success(new { imageUrl = url }, "Profile image updated successfully");
        }

        [HttpDelete("me/profile-image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            await _authService.DeleteProfileImageAsync(GetUserId());
            return SuccessMessage("Profile image removed successfully");
        }
    }
}
