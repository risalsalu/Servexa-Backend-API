using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/user-services")]
    [Authorize(Roles = "Customer")]
    public class UserServicesController : BaseController
    {
        private readonly IUserShopService _userShopService;

        public UserServicesController(IUserShopService userShopService)
        {
            _userShopService = userShopService;
        }

        private Guid GetCustomerId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet("shops")]
        public async Task<IActionResult> GetShops()
        {
            var result = (await _userShopService.GetShopsAsync(GetCustomerId())).ToList();

            if (!result.Any())
                return Success(result, "No shops are available near your current location. Please update your location and try again.");

            return Success(result, "Shops fetched successfully");
        }

        [HttpGet("shops/{shopId:guid}")]
        public async Task<IActionResult> GetShopServices(Guid shopId)
        {
            var result = await _userShopService.GetShopServicesAsync(GetCustomerId(), shopId);
            if (result == null)
                return NotFoundError("Shop not found");

            if (!result.IsActive)
                return Success(result, $"This shop is currently unavailable. Reason: {result.OfflineReason}");

            return Success(result, "Shop details fetched successfully");
        }
    }
}
