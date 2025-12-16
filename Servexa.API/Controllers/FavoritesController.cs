using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Favorites;
using Servexa.Application.Interfaces.Favorites;

namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/favorites")]
    public class FavoritesController : BaseController
    {
        private readonly IFavoriteService _service;

        public FavoritesController(IFavoriteService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDto dto)
        {
            var userId = GetUserId();
            var ok = await _service.AddFavoriteAsync(userId, dto);
            if (!ok) return Error("Service not found");
            return SuccessMessage("Service added to favorites successfully");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFavorite([FromBody] RemoveFavoriteDto dto)
        {
            var userId = GetUserId();
            var ok = await _service.RemoveFavoriteAsync(userId, dto);
            if (!ok) return Error("Favorite service not found");
            return SuccessMessage("Service removed from favorites successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            var result = await _service.GetUserFavoritesAsync(userId);
            return Success(result, "Favorites fetched successfully");
        }
    }
}
