using Microsoft.AspNetCore.Http;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Shared.Responses;
using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IShopImageRepository _shopImageRepository;
        private readonly ICloudinaryService _cloudinary;

        public ShopService(
            IShopRepository shopRepository,
            IShopImageRepository shopImageRepository,
            ICloudinaryService cloudinary)
        {
            _shopRepository = shopRepository;
            _shopImageRepository = shopImageRepository;
            _cloudinary = cloudinary;
        }

        public async Task<ApiResponse<Guid>> RegisterShopAsync(
            Guid ownerId,
            ShopUpsertRequest request,
            IFormFile shopImage,
            IFormFile licenseImage,
            IFormFile idProofImage)
        {
            var exists = await _shopRepository.OwnerHasShopAsync(ownerId);
            if (exists)
                return ApiResponse<Guid>.ErrorResponse("Shop already exists.");

            var shop = new Shop
            {
                OwnerId = ownerId,
                ShopName = request.ShopName,
                CategoryId = request.CategoryId,
                Description = request.Description,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Phone = request.Phone,
                HomeServiceAvailable = request.HomeServiceAvailable,
                WorkingHours = JsonSerializer.Serialize(request.WorkingHours),
                IsActive = false,
                OfflineReason = null
            };

            var shopId = await _shopRepository.CreateAsync(shop);

            if (shopImage != null)
            {
                var (url, publicId) = await _cloudinary.UploadAsync(shopImage);
                await _shopImageRepository.AddAsync(new ShopImage
                {
                    ShopId = shopId,
                    ImageUrl = url,
                    PublicId = publicId,
                    ImageType = "Shop"
                });
            }

            if (licenseImage != null)
            {
                var (url, publicId) = await _cloudinary.UploadAsync(licenseImage);
                await _shopImageRepository.AddAsync(new ShopImage
                {
                    ShopId = shopId,
                    ImageUrl = url,
                    PublicId = publicId,
                    ImageType = "License"
                });
            }

            if (idProofImage != null)
            {
                var (url, publicId) = await _cloudinary.UploadAsync(idProofImage);
                await _shopImageRepository.AddAsync(new ShopImage
                {
                    ShopId = shopId,
                    ImageUrl = url,
                    PublicId = publicId,
                    ImageType = "IdProof"
                });
            }

            return ApiResponse<Guid>.SuccessResponse(shopId);
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
                CategoryId = shop.CategoryId,
                Description = shop.Description,
                Address = shop.Address,
                Latitude = shop.Latitude,
                Longitude = shop.Longitude,
                Phone = shop.Phone,
                HomeServiceAvailable = shop.HomeServiceAvailable,
                WorkingHours = shop.WorkingHours,
                IsActive = shop.IsActive,
                OfflineReason = shop.OfflineReason,
                Images = images.Select(i => i.ImageUrl).ToList()
            };

            return ApiResponse<ShopResponseDto>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<bool>> UpdateShopAsync(
            Guid ownerId,
            ShopUpsertRequest request,
            IFormFile shopImage,
            IFormFile licenseImage,
            IFormFile idProofImage)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                return ApiResponse<bool>.ErrorResponse("Shop not found.");

            shop.ShopName = request.ShopName;
            shop.CategoryId = request.CategoryId;
            shop.Description = request.Description;
            shop.Address = request.Address;
            shop.Latitude = request.Latitude;
            shop.Longitude = request.Longitude;
            shop.Phone = request.Phone;
            shop.HomeServiceAvailable = request.HomeServiceAvailable;
            shop.WorkingHours = JsonSerializer.Serialize(request.WorkingHours);

            if (shopImage != null)
            {
                var (url, publicId) = await _cloudinary.UploadAsync(shopImage);
                await _shopImageRepository.UpdateExistingImageAsync(shop.Id, "Shop", url, publicId);
            }

            if (licenseImage != null)
            {
                var (url, publicId) = await _cloudinary.UploadAsync(licenseImage);
                await _shopImageRepository.UpdateExistingImageAsync(shop.Id, "License", url, publicId);
            }

            if (idProofImage != null)
            {
                var (url, publicId) = await _cloudinary.UploadAsync(idProofImage);
                await _shopImageRepository.UpdateExistingImageAsync(shop.Id, "IdProof", url, publicId);
            }

            await _shopRepository.UpdateAsync(shop);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<bool>> SetActiveStatusAsync(Guid ownerId, ActivateShopDto dto)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                return ApiResponse<bool>.ErrorResponse("Shop not found.");

            if (!dto.IsActive && string.IsNullOrWhiteSpace(dto.OfflineReason))
                return ApiResponse<bool>.ErrorResponse("Offline reason is required.");

            await _shopRepository.SetActiveStatusAsync(
                ownerId,
                dto.IsActive,
                dto.IsActive ? null : dto.OfflineReason
            );

            return ApiResponse<bool>.SuccessResponse(true, "Status Updated");
        }

        public async Task<ApiResponse<AddShopImageDto>> AddShopImageAsync(Guid ownerId, IFormFile file)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                return ApiResponse<AddShopImageDto>.ErrorResponse("Shop not found.");

            var (url, publicId) = await _cloudinary.UploadAsync(file);

            var image = await _shopImageRepository.AddAsync(new ShopImage
            {
                ShopId = shop.Id,
                ImageUrl = url,
                PublicId = publicId,
                ImageType = "Gallery"
            });

            return ApiResponse<AddShopImageDto>.SuccessResponse(new AddShopImageDto
            {
                ImageId = image.Id,
                ImageUrl = image.ImageUrl
            });
        }

        public async Task<ApiResponse<bool>> DeleteShopImageAsync(Guid ownerId, Guid imageId)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                return ApiResponse<bool>.ErrorResponse("Shop not found.");

            var image = await _shopImageRepository.GetByIdAsync(imageId);
            if (image == null || image.ShopId != shop.Id)
                return ApiResponse<bool>.ErrorResponse("Image not found.");

            await _cloudinary.DeleteAsync(image.PublicId);
            await _shopImageRepository.DeleteAsync(imageId);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<IEnumerable<ShopResponseDto>>> GetAllActiveShopsAsync()
        {
            var shops = await _shopRepository.GetActiveShopsAsync();

            var result = shops.Select(s => new ShopResponseDto
            {
                ShopId = s.Id,
                OwnerId = s.OwnerId,
                ShopName = s.ShopName,
                CategoryId = s.CategoryId,
                Description = s.Description,
                Address = s.Address,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Phone = s.Phone,
                HomeServiceAvailable = s.HomeServiceAvailable,
                WorkingHours = s.WorkingHours,
                IsActive = s.IsActive,
                OfflineReason = s.OfflineReason
            });

            return ApiResponse<IEnumerable<ShopResponseDto>>.SuccessResponse(result);
        }
    }
}
