using Microsoft.AspNetCore.Http;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
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
        private readonly IBookingRepository _bookingRepository;
        private readonly ICloudinaryService _cloudinary;

        public ShopService(
            IShopRepository shopRepository,
            IShopImageRepository shopImageRepository,
            IBookingRepository bookingRepository,
            ICloudinaryService cloudinary)
        {
            _shopRepository = shopRepository;
            _shopImageRepository = shopImageRepository;
            _bookingRepository = bookingRepository;
            _cloudinary = cloudinary;
        }

        public async Task<Guid> RegisterShopAsync(
            Guid ownerId,
            ShopUpsertRequest request,
            IFormFile? shopImage,
            IFormFile? licenseImage,
            IFormFile? idProofImage)
        {
            if (await _shopRepository.OwnerHasShopAsync(ownerId))
                throw new Exception("Shop already exists");

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
                    ImageType = ShopImageType.Shop
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
                    ImageType = ShopImageType.License
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
                    ImageType = ShopImageType.OwnerId
                });
            }

            return shopId;
        }

        public async Task<ShopResponseDto> GetShopAsync(Guid ownerId)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                throw new Exception("Shop not found");

            var images = await _shopImageRepository.GetByShopIdAsync(shop.Id);

            return new ShopResponseDto
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
                Images = images.Select(i => new ShopImageDto
                {
                    ImageId = i.Id,
                    ImageUrl = i.ImageUrl,
                    ImageType = (int)i.ImageType,
                    ImageTypeName = i.ImageType.ToString()
                }).ToList()
            };
        }

        public async Task<bool> UpdateShopAsync(
            Guid ownerId,
            ShopUpsertRequest request,
            IFormFile? shopImage,
            IFormFile? licenseImage,
            IFormFile? idProofImage)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                throw new Exception("Shop not found");

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
                await _shopImageRepository.UpdateExistingImageAsync(shop.Id, ShopImageType.Shop, url, publicId);
            }

            if (licenseImage != null)
            {
                var (url, publicId) = await _cloudinary.UploadAsync(licenseImage);
                await _shopImageRepository.UpdateExistingImageAsync(shop.Id, ShopImageType.License, url, publicId);
            }

            if (idProofImage != null)
            {
                var (url, publicId) = await _cloudinary.UploadAsync(idProofImage);
                await _shopImageRepository.UpdateExistingImageAsync(shop.Id, ShopImageType.OwnerId, url, publicId);
            }

            await _shopRepository.UpdateAsync(shop);
            return true;
        }

        public async Task<bool> SetActiveStatusAsync(Guid ownerId, ActivateShopDto dto)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                throw new Exception("Shop not found");

            if (!dto.IsActive)
            {
                if (string.IsNullOrWhiteSpace(dto.OfflineReason))
                    throw new Exception("Offline reason is required");

                var hasConfirmed = await _bookingRepository.HasConfirmedBookingsAsync(shop.Id);
                if (hasConfirmed)
                    throw new Exception("Cannot go offline while confirmed bookings exist");
            }

            await _shopRepository.SetActiveStatusAsync(
                ownerId,
                dto.IsActive,
                dto.IsActive ? null : dto.OfflineReason);

            return true;
        }

        public async Task<AddShopImageDto> AddShopImageAsync(
            Guid ownerId,
            IFormFile file,
            ShopImageType imageType)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                throw new Exception("Shop not found");

            var (url, publicId) = await _cloudinary.UploadAsync(file);

            var image = await _shopImageRepository.AddAsync(new ShopImage
            {
                ShopId = shop.Id,
                ImageUrl = url,
                PublicId = publicId,
                ImageType = imageType
            });

            return new AddShopImageDto
            {
                ShopId = shop.Id,
                ImageBase64 = string.Empty,
                ImageType = (int)image.ImageType
            };
        }

        public async Task<bool> DeleteShopImageAsync(Guid ownerId, Guid imageId)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(ownerId);
            if (shop == null)
                throw new Exception("Shop not found");

            var image = await _shopImageRepository.GetByIdAsync(imageId);
            if (image == null || image.ShopId != shop.Id)
                throw new Exception("Image not found");

            await _cloudinary.DeleteAsync(image.PublicId);
            await _shopImageRepository.DeleteAsync(imageId);
            return true;
        }

        public async Task<IEnumerable<ShopResponseDto>> GetAllActiveShopsAsync()
        {
            var shops = await _shopRepository.GetActiveShopsAsync();

            return shops.Select(s => new ShopResponseDto
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
        }
    }
}
