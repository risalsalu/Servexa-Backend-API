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
        private readonly ICustomerAddressRepository _customerAddressRepository;

        public UserShopService(
            IShopRepository shopRepository,
            IShopServiceRepository shopServiceRepository,
            IShopImageRepository shopImageRepository,
            ICustomerAddressRepository customerAddressRepository)
        {
            _shopRepository = shopRepository;
            _shopServiceRepository = shopServiceRepository;
            _shopImageRepository = shopImageRepository;
            _customerAddressRepository = customerAddressRepository;
        }

        public async Task<IEnumerable<UserShopListDto>> GetShopsAsync(Guid customerId)
        {
            var address = await _customerAddressRepository.GetActiveAddressAsync(customerId);
            if (address == null || !address.Lat.HasValue || !address.Lng.HasValue)
                return Enumerable.Empty<UserShopListDto>();

            var shops = await _shopRepository.GetNearbyShopsAsync(
                (decimal)address.Lat.Value,
                (decimal)address.Lng.Value,
                10
            );

            if (!shops.Any())
                return Enumerable.Empty<UserShopListDto>();

            var result = new List<UserShopListDto>();

            foreach (var s in shops)
            {
                var imageUrl = await _shopImageRepository.GetPrimaryImageUrlAsync(s.Id);

                result.Add(new UserShopListDto
                {
                    ShopId = s.Id,
                    ShopName = s.ShopName,
                    Address = s.Address,
                    ImageUrl = imageUrl,
                    IsActive = s.IsActive,
                    OfflineReason = s.OfflineReason
                });
            }

            return result;
        }

        public async Task<UserShopWithServicesDto?> GetShopServicesAsync(Guid customerId, Guid shopId)
        {
            var address = await _customerAddressRepository.GetActiveAddressAsync(customerId);
            if (address == null || !address.Lat.HasValue || !address.Lng.HasValue)
                return null;

            var shops = await _shopRepository.GetNearbyShopsAsync(
                (decimal)address.Lat.Value,
                (decimal)address.Lng.Value,
                10
            );

            var shop = shops.FirstOrDefault(s => s.Id == shopId);
            if (shop == null)
                return null;

            var services = shop.IsActive
                ? await _shopServiceRepository.GetActiveByShopAsync(shopId)
                : Enumerable.Empty<Domain.Models.ShopService>();

            return new UserShopWithServicesDto
            {
                ShopId = shop.Id,
                ShopName = shop.ShopName,
                IsActive = shop.IsActive,
                OfflineReason = shop.OfflineReason,
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
