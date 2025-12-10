using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Cart;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private Guid GetUserId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = GetUserId();
            var response = await _cartService.AddToCartAsync(userId, dto);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] Guid shopId)
        {
            var userId = GetUserId();
            var response = await _cartService.GetCartForShopAsync(userId, shopId);
            return Ok(response);
        }

        [HttpPatch("{cartItemId:guid}")]
        public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            var response = await _cartService.UpdateCartItemAsync(userId, cartItemId, dto);
            return Ok(response);
        }

        [HttpDelete("{cartItemId:guid}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            var userId = GetUserId();
            var response = await _cartService.RemoveCartItemAsync(userId, cartItemId);
            return Ok(response);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart([FromQuery] Guid shopId)
        {
            var userId = GetUserId();
            var response = await _cartService.ClearCartAsync(userId, shopId);
            return Ok(response);
        }
    }
}
