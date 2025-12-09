using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        private Guid GetUserId()
            => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = GetUserId();
            var result = await _authService.GetCurrentUserAsync(userId);
            return Success(result, "User profile fetched successfully");
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            var userId = GetUserId();
            await _authService.UpdateProfileAsync(userId, dto);
            return SuccessMessage("Profile updated successfully");
        }

        [HttpPatch("me/contact")]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactInfoDto dto)
        {
            var userId = GetUserId();
            await _authService.UpdateContactInfoAsync(userId, dto);
            return SuccessMessage("Contact information updated successfully");
        }

        [HttpPost("me/profile-image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] ProfileImageUploadDto dto)
        {
            var userId = GetUserId();
            var url = await _authService.UploadProfileImageAsync(userId, dto.File);

            var response = new ProfileImageDto
            {
                ImageUrl = url
            };

            return Success(response, "Profile image updated successfully");
        }

        [HttpDelete("me/profile-image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var userId = GetUserId();
            await _authService.DeleteProfileImageAsync(userId);
            return SuccessMessage("Profile image removed successfully");
        }
    }
}
