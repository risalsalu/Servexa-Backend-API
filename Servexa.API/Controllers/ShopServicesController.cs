using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Services;
using Servexa.Application.Interfaces;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/shop/services")]
    [Authorize(Roles = "ShopOwner")]
    public class ShopServicesController : BaseController
    {
        private readonly IShopServiceManagementService _service;
        private readonly IShopService _shopService;

        public ShopServicesController(
            IShopServiceManagementService service,
            IShopService shopService)
        {
            _service = service;
            _shopService = shopService;
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }

        private async Task<Guid?> GetOwnerShopIdAsync()
        {
            var ownerId = GetUserId();
            var result = await _shopService.GetShopAsync(ownerId);
            if (!result.Success || result.Data == null)
                return null;
            return result.Data.ShopId;
        }

        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] AddShopServiceDto dto)
        {
            var shopId = await GetOwnerShopIdAsync();
            if (shopId == null || shopId == Guid.Empty)
                return Error("Shop not found.");

            var result = await _service.AddServiceAsync(shopId.Value, dto);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }

        [HttpPut("{serviceId:guid}")]
        public async Task<IActionResult> UpdateService(Guid serviceId, [FromBody] UpdateShopServiceDto dto)
        {
            var shopId = await GetOwnerShopIdAsync();
            if (shopId == null || shopId == Guid.Empty)
                return Error("Shop not found.");

            var result = await _service.UpdateServiceAsync(shopId.Value, serviceId, dto);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }

        [HttpDelete("{serviceId:guid}")]
        public async Task<IActionResult> DeleteService(Guid serviceId)
        {
            var shopId = await GetOwnerShopIdAsync();
            if (shopId == null || shopId == Guid.Empty)
                return Error("Shop not found.");

            var result = await _service.DeleteServiceAsync(shopId.Value, serviceId, shopId.Value);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetOwnerServices()
        {
            var shopId = await GetOwnerShopIdAsync();
            if (shopId == null || shopId == Guid.Empty)
                return Error("Shop not found.");

            var result = await _service.GetServicesForOwnerAsync(shopId.Value);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }
    }
}
