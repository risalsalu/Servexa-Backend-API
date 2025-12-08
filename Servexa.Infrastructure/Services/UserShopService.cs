using Servexa.Application.DTOs.UserServices;
using Servexa.Application.Interfaces;

namespace Servexa.Application.Services
{
    public class UserShopService : IUserShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IShopServiceRepository _shopServiceRepository;

        public UserShopService(
            IShopRepository shopRepository,
            IShopServiceRepository shopServiceRepository)
        {
            _shopRepository = shopRepository;
            _shopServiceRepository = shopServiceRepository;
        }

        public async Task<IEnumerable<UserShopListDto>> GetActiveShopsAsync()
        {
            var shops = await _shopRepository.GetActiveShopsAsync();

            return shops.Select(s => new UserShopListDto
            {
                ShopId = s.Id,
                ShopName = s.ShopName,
                Address = s.Address
            });
        }

        public async Task<UserShopWithServicesDto?> GetShopServicesAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);

            if (shop == null || !shop.IsActive)
                return null;

            var services = await _shopServiceRepository.GetActiveByShopAsync(shopId);

            return new UserShopWithServicesDto
            {
                ShopId = shop.Id,
                ShopName = shop.ShopName,
                Services = services.Select(s => new UserServiceListDto
                {
                    ServiceId = s.Id,
                    Name = s.Name,
                    Price = s.Price,
                    DurationMinutes = s.DurationMinutes
                }).ToList()
            };
        }
    }
}
