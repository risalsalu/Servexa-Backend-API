using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Favorites;

namespace Servexa.Application.Interfaces.Favorites
{
    public interface IFavoriteService
    {
        Task<bool> AddFavoriteAsync(Guid userId, AddFavoriteDto dto);
        Task<bool> RemoveFavoriteAsync(Guid userId, RemoveFavoriteDto dto);
        Task<FavoriteServiceListDto> GetUserFavoritesAsync(Guid userId);
    }
}
