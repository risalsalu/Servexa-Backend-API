using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.Interfaces;
using System;
using System.Threading.Tasks;

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
            return Success(result, "Active shops fetched successfully");
        }

        [HttpGet("shops/{shopId:guid}")]
        public async Task<IActionResult> GetShopServices(Guid shopId)
        {
            var result = await _userShopService.GetShopServicesAsync(shopId);

            if (result == null)
                return Error("Shop not found or inactive");

            return Success(result, "Shop services fetched successfully");
        }
    }
}
