using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.Interfaces;

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

        [HttpGet("shops")]
        public async Task<IActionResult> GetShops()
        {
            var result = await _userShopService.GetShopsAsync();
            return Success(result, "Shops fetched successfully");
        }

        [HttpGet("shops/{shopId:guid}")]
        public async Task<IActionResult> GetShopServices(Guid shopId)
        {
            var result = await _userShopService.GetShopServicesAsync(shopId);
            if (result == null)
                return NotFoundError("Shop not found");

            return Success(result, "Shop details fetched successfully");
        }
    }
}
