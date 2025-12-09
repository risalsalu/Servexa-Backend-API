using Servexa.Application.DTOs.Favorites;
using Servexa.Application.Interfaces;
using Servexa.Application.Interfaces.Favorites;
using Servexa.Domain.Models;

namespace Servexa.Application.Services.Favorites
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepo;
        private readonly IShopServiceRepository _shopServiceRepo;

        public FavoriteService(
            IFavoriteRepository favoriteRepo,
            IShopServiceRepository shopServiceRepo)
        {
            _favoriteRepo = favoriteRepo;
            _shopServiceRepo = shopServiceRepo;
        }

        public async Task<bool> AddFavoriteAsync(Guid userId, AddFavoriteDto dto)
        {
            var exists = await _favoriteRepo.AnyAsync(x =>
                x.UserId == userId &&
                x.ShopServiceId == dto.ShopServiceId
            );

            if (exists) return true;

            var fav = new UserFavorite
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ShopServiceId = dto.ShopServiceId,
                CreatedOn = DateTime.UtcNow
            };

            await _favoriteRepo.AddAsync(fav);
            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(Guid userId, RemoveFavoriteDto dto)
        {
            var fav = await _favoriteRepo.GetOneAsync(x =>
                x.UserId == userId &&
                x.ShopServiceId == dto.ShopServiceId
            );

            if (fav == null) return false;

            return await _favoriteRepo.DeleteAsync(fav.Id);
        }

        public async Task<FavoriteServiceListDto> GetUserFavoritesAsync(Guid userId)
        {
            var all = await _favoriteRepo.GetAllAsync();
            var favorites = all.Where(x => x.UserId == userId).ToList();

            var items = new List<FavoriteServiceDto>();

            foreach (var fav in favorites)
            {
                var details = await _shopServiceRepo.GetServiceWithDetailsAsync(fav.ShopServiceId);
                if (details == null) continue;

                items.Add(new FavoriteServiceDto
                {
                    Id = fav.Id,
                    ShopServiceId = details.Id,
                    ServiceName = details.ServiceName,
                    CategoryName = details.CategoryName,
                    Price = details.Price,
                    DurationMinutes = details.DurationMinutes,
                    ShopId = details.ShopId,
                    ShopName = details.ShopName
                });
            }

            return new FavoriteServiceListDto
            {
                Items = items
            };
        }
    }
}
