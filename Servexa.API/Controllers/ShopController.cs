using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servexa.API.Models;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/shop")]
    [Authorize(Roles = "ShopOwner")]
    public class ShopController : BaseController
    {
        private readonly IShopService _shopService;
        private readonly ICloudinaryService _cloudinary;
        private readonly IAdminCategoryService _categoryService;

        public ShopController(
            IShopService shopService,
            ICloudinaryService cloudinary,
            IAdminCategoryService categoryService)
        {
            _shopService = shopService;
            _cloudinary = cloudinary;
            _categoryService = categoryService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] AddShopRequest req)
        {
            var ownerId = GetUserId();

            var dto = new AddShopDto
            {
                ShopName = req.ShopName,
                CategoryId = req.CategoryId,
                Description = req.Description,
                Address = req.Address,
                Latitude = req.Latitude,
                Longitude = req.Longitude,
                Phone = req.Phone,
                HomeServiceAvailable = req.HomeServiceAvailable,
                WorkingHours = JsonSerializer.Deserialize<WorkingHoursDto>(req.WorkingHoursJson)
            };

            var (shopUrl, shopPublicId) = await _cloudinary.UploadAsync(req.ShopImage);
            var (licenseUrl, licensePublicId) = await _cloudinary.UploadAsync(req.LicenseImage);
            var (idUrl, idPublicId) = await _cloudinary.UploadAsync(req.IdProofImage);

            var result = await _shopService.RegisterShopAsync(
                ownerId,
                dto,
                shopUrl,
                shopPublicId,
                licenseUrl,
                licensePublicId,
                idUrl,
                idPublicId
            );

            if (!result.Success)
                return Error(result.Message);

            return Success(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ownerId = GetUserId();
            var result = await _shopService.GetShopAsync(ownerId);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateShopDto dto)
        {
            var ownerId = GetUserId();
            var result = await _shopService.UpdateShopAsync(ownerId, dto);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }

        [HttpPatch("activate")]
        public async Task<IActionResult> Activate([FromBody] ActivateShopDto dto)
        {
            var ownerId = GetUserId();
            var result = await _shopService.SetActiveStatusAsync(ownerId, dto.IsActive);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }

        [HttpPost("images")]
        public async Task<IActionResult> AddImage(IFormFile file)
        {
            var ownerId = GetUserId();
            var result = await _shopService.AddShopImageAsync(ownerId, file);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }

        [HttpDelete("images/{imageId:guid}")]
        public async Task<IActionResult> DeleteImage(Guid imageId)
        {
            var ownerId = GetUserId();
            var result = await _shopService.DeleteShopImageAsync(ownerId, imageId);
            if (!result.Success)
                return Error(result.Message);
            return Success(result.Data);
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
