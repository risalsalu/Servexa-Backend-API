using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface IShopImageRepository
    {
        Task<ShopImage> AddAsync(ShopImage image);
        Task<ShopImage?> GetByIdAsync(Guid id);
        Task<List<ShopImage>> GetByShopIdAsync(Guid shopId);
        Task DeleteAsync(Guid id);
        Task<string?> GetPrimaryImageUrlAsync(Guid shopId);
        Task UpdateExistingImageAsync(Guid shopId, string imageType, string imageUrl, string publicId);
    }
}
