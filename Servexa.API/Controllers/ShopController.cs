using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/shop")]
    [Authorize(Roles = "ShopOwner")]
    public class ShopController : BaseController
    {
        private readonly IShopService _shopService;
        private readonly IAdminCategoryService _categoryService;

        public ShopController(
            IShopService shopService,
            IAdminCategoryService categoryService)
        {
            _shopService = shopService;
            _categoryService = categoryService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetAllAsync();
            return Success(result, "Categories fetched successfully");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ShopUpsertRequest request)
        {
            var shopId = await _shopService.RegisterShopAsync(
                GetUserId(),
                request,
                null,
                null,
                null);

            return Created(shopId, "Shop registered successfully");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _shopService.GetShopAsync(GetUserId());
            return Success(result, "Shop fetched successfully");
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ShopUpsertRequest request)
        {
            var result = await _shopService.UpdateShopAsync(
                GetUserId(),
                request,
                null,
                null,
                null);

            return Success(result, "Shop updated successfully");
        }

        [HttpPatch("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] ActivateShopDto dto)
        {
            var result = await _shopService.SetActiveStatusAsync(GetUserId(), dto);
            return Success(result, "Shop status updated");
        }

        [HttpPost("images")]
        public async Task<IActionResult> AddImage(
            IFormFile file,
            [FromQuery] ShopImageType imageType)
        {
            var result = await _shopService.AddShopImageAsync(GetUserId(), file, imageType);
            return Success(result, "Image added successfully");
        }

        [HttpDelete("images/{imageId:guid}")]
        public async Task<IActionResult> DeleteImage(Guid imageId)
        {
            var result = await _shopService.DeleteShopImageAsync(GetUserId(), imageId);
            return Success(result, "Image deleted successfully");
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
