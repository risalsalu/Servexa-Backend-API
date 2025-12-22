using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Cart;

namespace Servexa.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto?> AddToCartAsync(Guid userId, AddToCartDto dto);
        Task<CartResponseDto?> GetCartForShopAsync(Guid userId, Guid shopId);
        Task<CartResponseDto?> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto);
        Task<bool> RemoveCartItemAsync(Guid userId, Guid cartItemId);
        Task<bool> ClearCartAsync(Guid userId, Guid shopId);
    }
}
