using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Servexa.Application.DTOs.Shop;
using Servexa.Shared.Responses;

namespace Servexa.Application.Interfaces
{
    public interface IShopService
    {
        Task<ApiResponse<Guid>> RegisterShopAsync(
            Guid ownerId,
            ShopUpsertRequest request,
            IFormFile shopImage,
            IFormFile licenseImage,
            IFormFile idProofImage);

        Task<ApiResponse<ShopResponseDto>> GetShopAsync(Guid ownerId);

        Task<ApiResponse<bool>> UpdateShopAsync(
            Guid ownerId,
            ShopUpsertRequest request,
            IFormFile shopImage,
            IFormFile licenseImage,
            IFormFile idProofImage);

        Task<ApiResponse<bool>> SetActiveStatusAsync(Guid ownerId, bool isActive);

        Task<ApiResponse<AddShopImageDto>> AddShopImageAsync(Guid ownerId, IFormFile file);

        Task<ApiResponse<bool>> DeleteShopImageAsync(Guid ownerId, Guid imageId);

        Task<ApiResponse<IEnumerable<ShopResponseDto>>> GetAllActiveShopsAsync();
    }
}
