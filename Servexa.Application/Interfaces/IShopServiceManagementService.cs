using Servexa.Application.DTOs.Services;
using Servexa.Shared.Responses;

namespace Servexa.Application.Interfaces
{
    public interface IShopServiceManagementService
    {
        Task<ApiResponse<ShopServiceResponseDto>> AddServiceAsync(Guid shopId, AddShopServiceDto dto);
        Task<ApiResponse<ShopServiceResponseDto>> UpdateServiceAsync(Guid shopId, Guid serviceId, UpdateShopServiceDto dto);
        Task<ApiResponse<bool>> DeleteServiceAsync(Guid shopId, Guid serviceId, Guid deletedBy);
        Task<ApiResponse<IEnumerable<ShopServiceResponseDto>>> GetServicesForOwnerAsync(Guid shopId);
        Task<ApiResponse<IEnumerable<ShopServiceListItemDto>>> GetServicesForUserAsync(Guid shopId);
    }
}
