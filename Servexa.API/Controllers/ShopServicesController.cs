using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Services;
using Servexa.Application.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

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
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private async Task<Guid?> GetOwnerShopIdAsync()
        {
            var shop = await _shopService.GetShopAsync(GetUserId());
            return shop.ShopId;
        }

        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] AddShopServiceDto dto)
        {
            var shopId = await GetOwnerShopIdAsync();
            if (shopId == null)
                return NotFoundError("Shop not found");

            var result = await _service.AddServiceAsync(shopId.Value, dto);
            return Created(result, "Service added successfully");
        }

        [HttpPut("{serviceId:guid}")]
        public async Task<IActionResult> UpdateService(Guid serviceId, [FromBody] UpdateShopServiceDto dto)
        {
            var shopId = await GetOwnerShopIdAsync();
            if (shopId == null)
                return NotFoundError("Shop not found");

            var result = await _service.UpdateServiceAsync(shopId.Value, serviceId, dto);
            return Success(result, "Service updated successfully");
        }

        [HttpDelete("{serviceId:guid}")]
        public async Task<IActionResult> DeleteService(Guid serviceId)
        {
            var shopId = await GetOwnerShopIdAsync();
            if (shopId == null)
                return NotFoundError("Shop not found");

            await _service.DeleteServiceAsync(shopId.Value, serviceId, shopId.Value);
            return SuccessMessage("Service deleted successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetOwnerServices()
        {
            var shopId = await GetOwnerShopIdAsync();
            if (shopId == null)
                return NotFoundError("Shop not found");

            var result = await _service.GetServicesForOwnerAsync(shopId.Value);
            return Success(result, "Services fetched successfully");
        }
    }
}
