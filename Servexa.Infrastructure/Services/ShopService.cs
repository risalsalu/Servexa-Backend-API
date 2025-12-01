using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Shared.Responses;

namespace Servexa.Infrastructure.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IShopImageRepository _shopImageRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public ShopService(
            IShopRepository shopRepository,
            IShopImageRepository shopImageRepository,
            ICloudinaryService cloudinaryService)
        {
            _shopRepository = shopRepository;
            _shopImageRepository = shopImageRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<Guid>> RegisterShopAsync(Guid ownerId, AddShopDto dto)
        {
            var exists = await _shopRepository.OwnerHasShopAsync(ownerId);
            if (exists)
                return ApiResponse<Guid>.ErrorResponse("Shop already exists for this owner.");

            var shop = new Shop
            {
                OwnerId = ownerId,
                ShopName = dto.ShopName,
                Categories = dto.Categories,
                Description = dto.Description,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Phone = dto.Phone,
                HomeServiceAvailable = dto.HomeServiceAvailable,
                LicenseImageUrl = dto.LicenseImageUrl,
                IdProofImageUrl = dto.IdProofImageUrl,
                Services = dto.Services,
                WorkingHours = dto.WorkingHours,
                IsActive = false
            };

            var id = await _shopRepository.CreateAsync(shop);

            return ApiResponse<Guid>.SuccessResponse(id, "Shop registered successfully.");
        }

        public async Task<ApiResponse<ShopResponseDto>> GetShopAsync(Guid ownerId)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                return ApiResponse<ShopResponseDto>.ErrorResponse("Shop not found.");

            var images = await _shopImageRepository.GetByShopIdAsync(shop.Id);

            var dto = new ShopResponseDto
            {
                ShopId = shop.Id,
                OwnerId = shop.OwnerId,
                ShopName = shop.ShopName,
                Categories = shop.Categories,
                Description = shop.Description,
                Address = shop.Address,
                Latitude = shop.Latitude,
                Longitude = shop.Longitude,
                Phone = shop.Phone,
                HomeServiceAvailable = shop.HomeServiceAvailable,
                LicenseImageUrl = shop.LicenseImageUrl,
                IdProofImageUrl = shop.IdProofImageUrl,
                IsActive = shop.IsActive,
                Services = shop.Services,
                WorkingHours = shop.WorkingHours,
                Images = images.Select(i => i.ImageUrl).ToList()
            };

            return ApiResponse<ShopResponseDto>.SuccessResponse(dto, "Shop fetched successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateShopAsync(Guid ownerId, UpdateShopDto dto)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                return ApiResponse<bool>.ErrorResponse("Shop not found.");

            shop.ShopName = dto.ShopName;
            shop.Categories = dto.Categories;
            shop.Description = dto.Description;
            shop.Address = dto.Address;
            shop.Latitude = dto.Latitude;
            shop.Longitude = dto.Longitude;
            shop.Phone = dto.Phone;
            shop.HomeServiceAvailable = dto.HomeServiceAvailable;
            shop.Services = dto.Services;
            shop.WorkingHours = dto.WorkingHours;

            await _shopRepository.UpdateAsync(shop);

            return ApiResponse<bool>.SuccessResponse(true, "Shop updated successfully.");
        }

        public async Task<ApiResponse<bool>> SetActiveStatusAsync(Guid ownerId, bool isActive)
        {
            var exists = await _shopRepository.OwnerHasShopAsync(ownerId);
            if (!exists)
                return ApiResponse<bool>.ErrorResponse("Shop not found.");

            await _shopRepository.SetActiveStatusAsync(ownerId, isActive);

            return ApiResponse<bool>.SuccessResponse(true, "Shop status updated successfully.");
        }

        public async Task<ApiResponse<AddShopImageDto>> AddShopImageAsync(Guid ownerId, IFormFile file)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                return ApiResponse<AddShopImageDto>.ErrorResponse("Shop not found.");

            if (file == null || file.Length == 0)
                return ApiResponse<AddShopImageDto>.ErrorResponse("File is required.");

            var url = await _cloudinaryService.UploadAsync(file);

            var image = new ShopImage
            {
                ShopId = shop.Id,
                ImageUrl = url
            };

            image = await _shopImageRepository.AddAsync(image);

            var dto = new AddShopImageDto
            {
                ImageId = image.Id,
                ImageUrl = image.ImageUrl
            };

            return ApiResponse<AddShopImageDto>.SuccessResponse(dto, "Image uploaded successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteShopImageAsync(Guid ownerId, Guid imageId)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                return ApiResponse<bool>.ErrorResponse("Shop not found.");

            var image = await _shopImageRepository.GetByIdAsync(imageId);
            if (image == null || image.ShopId != shop.Id)
                return ApiResponse<bool>.ErrorResponse("Image not found.");

            await _shopImageRepository.DeleteAsync(imageId);

            return ApiResponse<bool>.SuccessResponse(true, "Image deleted successfully.");
        }
    }
}
