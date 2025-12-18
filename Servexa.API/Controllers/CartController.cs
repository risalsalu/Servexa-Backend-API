using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Cart;
using Servexa.Application.Interfaces;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/cart")]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var result = await _cartService.AddToCartAsync(GetUserId(), dto);
            return Success(result, "Added to cart");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] Guid shopId)
        {
            var result = await _cartService.GetCartForShopAsync(GetUserId(), shopId);
            return Success(result, "Cart fetched");
        }

        [HttpPatch("{cartItemId:guid}")]
        public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            var result = await _cartService.UpdateCartItemAsync(GetUserId(), cartItemId, dto);
            return Success(result, "Cart updated");
        }

        [HttpDelete("{cartItemId:guid}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(GetUserId(), cartItemId);
            return Success(result, "Item removed");
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart([FromQuery] Guid shopId)
        {
            var result = await _cartService.ClearCartAsync(GetUserId(), shopId);
            return Success(result, "Cart cleared");
        }
    }
}
