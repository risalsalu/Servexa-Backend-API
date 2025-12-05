using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Auth.Common;
using Servexa.Application.Interfaces;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IShopService _shopService;

        public UsersController(IAuthService authService, IShopService shopService)
        {
            _authService = authService;
            _shopService = shopService;
        }

        private Guid GetUserId()
            => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = GetUserId();
            var profile = await _authService.GetCurrentUserAsync(userId);
            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            var userId = GetUserId();
            await _authService.UpdateProfileAsync(userId, dto);
            return Ok(new { Message = "Profile updated" });
        }

        [HttpGet("shops")]
        public async Task<IActionResult> GetActiveShops()
        {
            var shops = await _shopService.GetAllActiveShopsAsync();
            return Ok(shops);
        }
    }
}
