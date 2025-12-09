using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Favorites;
using Servexa.Application.Interfaces.Favorites;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/favorites")]
    public class FavoritesController : BaseController
    {
        private readonly IFavoriteService _service;

        public FavoritesController(IFavoriteService service)
        {
            _service = service;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDto dto)
        {
            var id = GetUserId();
            await _service.AddFavoriteAsync(id, dto);
            return SuccessMessage("Service added to favorites successfully");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFavorite([FromBody] RemoveFavoriteDto dto)
        {
            var id = GetUserId();
            await _service.RemoveFavoriteAsync(id, dto);
            return SuccessMessage("Service removed from favorites successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var id = GetUserId();
            var result = await _service.GetUserFavoritesAsync(id);
            return Success(result, "Favorites fetched successfully");
        }
    }
}
