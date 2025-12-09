using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Users;
using Servexa.Application.Interfaces;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var id = GetUserId();
            var result = await _authService.GetCurrentUserAsync(id);
            return Success(result, "User profile fetched successfully");
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            var id = GetUserId();
            await _authService.UpdateProfileAsync(id, dto);
            return SuccessMessage("Profile updated successfully");
        }

        [HttpPatch("me/contact")]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactInfoDto dto)
        {
            var id = GetUserId();
            await _authService.UpdateContactInfoAsync(id, dto);
            return SuccessMessage("Contact information updated successfully");
        }

        [HttpPost("me/profile-image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] ProfileImageUploadDto dto)
        {
            var id = GetUserId();
            var url = await _authService.UploadProfileImageAsync(id, dto.File);
            return Success(new { imageUrl = url }, "Profile image updated successfully");
        }

        [HttpDelete("me/profile-image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var id = GetUserId();
            await _authService.DeleteProfileImageAsync(id);
            return SuccessMessage("Profile image removed successfully");
        }
    }
}
