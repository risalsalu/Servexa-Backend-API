using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Servexa.Application.DTOs.UserServices;
using Servexa.Application.Interfaces;

namespace Servexa.Application.Services
{
    public class UserShopService : IUserShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IShopServiceRepository _shopServiceRepository;
        private readonly IShopImageRepository _shopImageRepository;

        public UserShopService(
            IShopRepository shopRepository,
            IShopServiceRepository shopServiceRepository,
            IShopImageRepository shopImageRepository)
        {
            _shopRepository = shopRepository;
            _shopServiceRepository = shopServiceRepository;
            _shopImageRepository = shopImageRepository;
        }

        public async Task<IEnumerable<UserShopListDto>> GetActiveShopsAsync()
        {
            var shops = await _shopRepository.GetActiveShopsAsync();
            var result = new List<UserShopListDto>();

            foreach (var s in shops)
            {
                var imageUrl = await _shopImageRepository.GetPrimaryImageUrlAsync(s.Id);

                result.Add(new UserShopListDto
                {
                    ShopId = s.Id,
                    ShopName = s.ShopName,
                    Address = s.Address,
                    ImageUrl = imageUrl
                });
            }

            return result;
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
