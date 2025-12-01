using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/shop")]
    [Authorize(Roles = "ShopOwner")]
    public class ShopController : BaseController
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddShopDto dto)
        {
            var ownerId = GetUserId();
            var result = await _shopService.RegisterShopAsync(ownerId, dto);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ownerId = GetUserId();
            var result = await _shopService.GetShopAsync(ownerId);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateShopDto dto)
        {
            var ownerId = GetUserId();
            var result = await _shopService.UpdateShopAsync(ownerId, dto);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, result.Message);
        }

        [HttpPatch("activate")]
        public async Task<IActionResult> Activate([FromBody] ActivateShopDto dto)
        {
            var ownerId = GetUserId();
            var result = await _shopService.SetActiveStatusAsync(ownerId, dto.IsActive);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, result.Message);
        }

        [HttpPost("images")]
        public async Task<IActionResult> AddImage(IFormFile file)
        {
            var ownerId = GetUserId();
            var result = await _shopService.AddShopImageAsync(ownerId, file);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, result.Message);
        }

        [HttpDelete("images/{imageId:guid}")]
        public async Task<IActionResult> DeleteImage(Guid imageId)
        {
            var ownerId = GetUserId();
            var result = await _shopService.DeleteShopImageAsync(ownerId, imageId);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, result.Message);
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
