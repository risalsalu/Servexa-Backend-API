using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Register(
            [FromForm] ShopUpsertRequest request,
            IFormFile? shopImage,
            IFormFile? licenseImage,
            IFormFile? idProofImage)
        {
            var ownerId = GetUserId();

            var result = await _shopService.RegisterShopAsync(
                ownerId,
                request,
                shopImage,
                licenseImage,
                idProofImage);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, "Shop registered successfully");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ownerId = GetUserId();
            var result = await _shopService.GetShopAsync(ownerId);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, "Shop fetched successfully");
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            [FromForm] ShopUpsertRequest request,
            IFormFile? shopImage,
            IFormFile? licenseImage,
            IFormFile? idProofImage)
        {
            var ownerId = GetUserId();

            var result = await _shopService.UpdateShopAsync(
                ownerId,
                request,
                shopImage,
                licenseImage,
                idProofImage);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, "Shop updated successfully");
        }

        [HttpPatch("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] ActivateShopDto dto)
        {
            var ownerId = GetUserId();
            var result = await _shopService.SetActiveStatusAsync(ownerId, dto);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, "Shop status updated");
        }

        [HttpPost("images")]
        public async Task<IActionResult> AddImage(
            IFormFile file,
            [FromQuery] ShopImageType imageType)
        {
            var ownerId = GetUserId();
            var result = await _shopService.AddShopImageAsync(ownerId, file, imageType);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, "Image added successfully");
        }

        [HttpDelete("images/{imageId:guid}")]
        public async Task<IActionResult> DeleteImage(Guid imageId)
        {
            var ownerId = GetUserId();
            var result = await _shopService.DeleteShopImageAsync(ownerId, imageId);

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data, "Image deleted successfully");
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
