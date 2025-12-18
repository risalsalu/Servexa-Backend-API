using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Favorites;
using Servexa.Application.Interfaces.Favorites;
using System.Security.Claims;

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
            var ok = await _service.AddFavoriteAsync(GetUserId(), dto);
            if (!ok)
                return NotFoundError("Service not found");

            return SuccessMessage("Added to favorites");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFavorite([FromBody] RemoveFavoriteDto dto)
        {
            var ok = await _service.RemoveFavoriteAsync(GetUserId(), dto);
            if (!ok)
                return NotFoundError("Favorite not found");

            return SuccessMessage("Removed from favorites");
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var result = await _service.GetUserFavoritesAsync(GetUserId());
            return Success(result, "Favorites fetched");
        }
    }
}
