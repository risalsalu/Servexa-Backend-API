using Servexa.Application.DTOs.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces
{
    public interface IShopServiceManagementService
    {
        Task<ShopServiceResponseDto> AddServiceAsync(Guid shopId, AddShopServiceDto dto);
        Task<ShopServiceResponseDto> UpdateServiceAsync(Guid shopId, Guid serviceId, UpdateShopServiceDto dto);
        Task<bool> DeleteServiceAsync(Guid shopId, Guid serviceId, Guid deletedBy);
        Task<IEnumerable<ShopServiceResponseDto>> GetServicesForOwnerAsync(Guid shopId);
        Task<IEnumerable<ShopServiceListItemDto>> GetServicesForUserAsync(Guid shopId);
    }
}
