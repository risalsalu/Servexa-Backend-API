using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/user/services")]
    [AllowAnonymous]
    public class UserServicesController : BaseController
    {
        private readonly IUserShopService _userShopService;

        public UserServicesController(IUserShopService userShopService)
        {
            _userShopService = userShopService;
        }

        [HttpGet("shops")]
        public async Task<IActionResult> GetActiveShops()
        {
            var result = await _userShopService.GetActiveShopsAsync();
            return Success(new
            {
                Message = "Active shops fetched successfully",
                Data = result
            });
        }

        [HttpGet("shops/{shopId:guid}")]
        public async Task<IActionResult> GetShopServices(Guid shopId)
        {
            var result = await _userShopService.GetShopServicesAsync(shopId);

            if (result == null)
                return Error("Shop not found or inactive");

            return Success(new
            {
                Message = "Shop services fetched successfully",
                Data = result
            });
        }
    }
}
