using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Servexa.Application.DTOs.Shop;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface IShopService
    {
        Task<Guid> RegisterShopAsync(
            Guid ownerId,
            ShopUpsertRequest request,
            IFormFile? shopImage,
            IFormFile? licenseImage,
            IFormFile? idProofImage);

        Task<ShopResponseDto> GetShopAsync(Guid ownerId);

        Task<bool> UpdateShopAsync(
            Guid ownerId,
            ShopUpsertRequest request,
            IFormFile? shopImage,
            IFormFile? licenseImage,
            IFormFile? idProofImage);

        Task<bool> SetActiveStatusAsync(Guid ownerId, ActivateShopDto dto);

        Task<AddShopImageDto> AddShopImageAsync(
            Guid ownerId,
            IFormFile file,
            ShopImageType imageType);

        Task<bool> DeleteShopImageAsync(Guid ownerId, Guid imageId);

        Task<IEnumerable<ShopResponseDto>> GetAllActiveShopsAsync();
    }
}
