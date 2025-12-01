using Microsoft.AspNetCore.Http;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Shared.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Services;

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
        AddShopDto dto,
        string? shopUrl,
        string? shopPublicId,
        string? licenseUrl,
        string? licensePublicId,
        string? idUrl,
        string? idPublicId)
    {
        var exists = await _shopRepository.OwnerHasShopAsync(ownerId);
        if (exists)
            return ApiResponse<Guid>.ErrorResponse("Shop already exists.");

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
            Services = dto.Services,
            WorkingHours = dto.WorkingHours,
            IsActive = false
        };

        var shopId = await _shopRepository.CreateAsync(shop);

        if (shopUrl != null)
            await _shopImageRepository.AddAsync(new ShopImage
            {
                ShopId = shopId,
                ImageUrl = shopUrl,
                PublicId = shopPublicId ?? "",
                ImageType = "Shop"
            });

        if (licenseUrl != null)
            await _shopImageRepository.AddAsync(new ShopImage
            {
                ShopId = shopId,
                ImageUrl = licenseUrl,
                PublicId = licensePublicId ?? "",
                ImageType = "License"
            });

        if (idUrl != null)
            await _shopImageRepository.AddAsync(new ShopImage
            {
                ShopId = shopId,
                ImageUrl = idUrl,
                PublicId = idPublicId ?? "",
                ImageType = "IdProof"
            });

        return ApiResponse<Guid>.SuccessResponse(shopId, "Shop registered.");
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
            Services = shop.Services,
            WorkingHours = shop.WorkingHours,
            IsActive = shop.IsActive,
            Images = images.Select(i => i.ImageUrl).ToList()
        };

        return ApiResponse<ShopResponseDto>.SuccessResponse(dto, "Shop fetched.");
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

        return ApiResponse<bool>.SuccessResponse(true, "Updated.");
    }

    public async Task<ApiResponse<bool>> SetActiveStatusAsync(Guid ownerId, bool isActive)
    {
        var exists = await _shopRepository.OwnerHasShopAsync(ownerId);
        if (!exists)
            return ApiResponse<bool>.ErrorResponse("Shop not found.");

        await _shopRepository.SetActiveStatusAsync(ownerId, isActive);

        return ApiResponse<bool>.SuccessResponse(true, "Updated.");
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

        var dto = new AddShopImageDto
        {
            ImageId = image.Id,
            ImageUrl = image.ImageUrl
        };

        return ApiResponse<AddShopImageDto>.SuccessResponse(dto, "Uploaded.");
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

        return ApiResponse<bool>.SuccessResponse(true, "Deleted.");
    }
}
