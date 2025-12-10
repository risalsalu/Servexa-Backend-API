using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Cart;
using Servexa.Shared.Responses;

namespace Servexa.Application.Interfaces
{
    public interface ICartService
    {
        Task<ApiResponse<CartResponseDto>> AddToCartAsync(Guid userId, AddToCartDto dto);
        Task<ApiResponse<CartResponseDto>> GetCartForShopAsync(Guid userId, Guid shopId);
        Task<ApiResponse<CartResponseDto>> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto);
        Task<ApiResponse<bool>> RemoveCartItemAsync(Guid userId, Guid cartItemId);
        Task<ApiResponse<bool>> ClearCartAsync(Guid userId, Guid shopId);
    }
}
