using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Cart;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IShopServiceRepository _shopServiceRepository;

        public CartService(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IShopServiceRepository shopServiceRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _shopServiceRepository = shopServiceRepository;
        }

        public async Task<CartResponseDto?> AddToCartAsync(Guid userId, AddToCartDto dto)
        {
            var shopService = await _shopServiceRepository.GetByIdAsync(dto.ShopServiceId);
            if (shopService == null || shopService.ShopId != dto.ShopId)
                throw new Exception("Invalid shop or service");

            var cart = await _cartRepository.GetActiveCartForUserAndShopAsync(userId, dto.ShopId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    ShopId = dto.ShopId
                };
                await _cartRepository.AddAsync(cart);
            }

            var existingItem = await _cartItemRepository.GetByCartAndServiceAsync(
                cart.Id,
                dto.ShopServiceId,
                dto.SelectedDateTime);

            if (existingItem == null)
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ShopServiceId = dto.ShopServiceId,
                    Quantity = dto.Quantity,
                    Price = shopService.Price,
                    DurationMinutes = shopService.DurationMinutes,
                    SelectedDateTime = dto.SelectedDateTime
                };
                await _cartItemRepository.AddAsync(newItem);
            }
            else
            {
                existingItem.Quantity += dto.Quantity;
                existingItem.ModifiedOn = DateTime.UtcNow;
                await _cartItemRepository.UpdateAsync(existingItem);
            }

            var items = await _cartItemRepository.GetItemsByCartIdAsync(cart.Id);
            return await BuildCartResponseDto(cart, items);
        }

        public async Task<CartResponseDto?> GetCartForShopAsync(Guid userId, Guid shopId)
        {
            var cart = await _cartRepository.GetActiveCartForUserAndShopAsync(userId, shopId);
            if (cart == null)
                return null;

            var items = await _cartItemRepository.GetItemsByCartIdAsync(cart.Id);
            return await BuildCartResponseDto(cart, items);
        }

        public async Task<CartResponseDto?> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (cartItem == null)
                throw new Exception("Cart item not found");

            var cart = await _cartRepository.GetByIdAsync(cartItem.CartId);
            if (cart == null || cart.UserId != userId)
                throw new Exception("Cart not found");

            if (dto.Quantity.HasValue)
                cartItem.Quantity = dto.Quantity.Value;

            if (dto.SelectedDateTime.HasValue)
                cartItem.SelectedDateTime = dto.SelectedDateTime.Value;

            if (cartItem.Quantity <= 0)
            {
                await _cartItemRepository.DeleteAsync(cartItemId);
            }
            else
            {
                cartItem.ModifiedOn = DateTime.UtcNow;
                await _cartItemRepository.UpdateAsync(cartItem);
            }

            var items = await _cartItemRepository.GetItemsByCartIdAsync(cart.Id);
            if (!items.Any())
            {
                await _cartRepository.DeleteAsync(cart.Id);
                return null;
            }

            return await BuildCartResponseDto(cart, items);
        }

        public async Task<bool> RemoveCartItemAsync(Guid userId, Guid cartItemId)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (cartItem == null)
                throw new Exception("Cart item not found");

            var cart = await _cartRepository.GetByIdAsync(cartItem.CartId);
            if (cart == null || cart.UserId != userId)
                throw new Exception("Cart not found");

            await _cartItemRepository.DeleteAsync(cartItemId);

            var items = await _cartItemRepository.GetItemsByCartIdAsync(cart.Id);
            if (!items.Any())
                await _cartRepository.DeleteAsync(cart.Id);

            return true;
        }

        public async Task<bool> ClearCartAsync(Guid userId, Guid shopId)
        {
            var cart = await _cartRepository.GetActiveCartForUserAndShopAsync(userId, shopId);
            if (cart == null)
                return true;

            var items = await _cartItemRepository.GetItemsByCartIdAsync(cart.Id);
            foreach (var item in items)
                await _cartItemRepository.DeleteAsync(item.Id);

            await _cartRepository.DeleteAsync(cart.Id);
            return true;
        }

        private async Task<CartResponseDto> BuildCartResponseDto(Cart cart, IEnumerable<CartItem> items)
        {
            var list = new List<CartItemResponseDto>();
            decimal totalPrice = 0;
            int totalDuration = 0;

            foreach (var item in items)
            {
                var shopService = await _shopServiceRepository.GetByIdAsync(item.ShopServiceId);

                list.Add(new CartItemResponseDto
                {
                    CartItemId = item.Id,
                    ShopServiceId = item.ShopServiceId,
                    ServiceName = shopService?.Name ?? string.Empty,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    DurationMinutes = item.DurationMinutes,
                    SelectedDateTime = item.SelectedDateTime
                });

                totalPrice += item.Price * item.Quantity;
                totalDuration += item.DurationMinutes * item.Quantity;
            }

            return new CartResponseDto
            {
                CartId = cart.Id,
                ShopId = cart.ShopId,
                Items = list,
                TotalPrice = totalPrice,
                TotalDurationMinutes = totalDuration
            };
        }
    }
}
