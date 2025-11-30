using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Servexa.Application.DTOs.Shop;
using Servexa.Shared.Responses;

namespace Servexa.Application.Interfaces;

public interface IShopService
{
    Task<ApiResponse<Guid>> RegisterShopAsync(Guid ownerId, AddShopDto dto);
    Task<ApiResponse<ShopResponseDto>> GetShopAsync(Guid ownerId);
    Task<ApiResponse<bool>> UpdateShopAsync(Guid ownerId, UpdateShopDto dto);
    Task<ApiResponse<bool>> SetActiveStatusAsync(Guid ownerId, bool isActive);
    Task<ApiResponse<AddShopImageDto>> AddShopImageAsync(Guid ownerId, IFormFile file);
    Task<ApiResponse<bool>> DeleteShopImageAsync(Guid ownerId, Guid imageId);
}
