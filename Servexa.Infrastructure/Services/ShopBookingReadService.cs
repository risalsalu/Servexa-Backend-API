using Servexa.Application.DTOs.Booking;
using Servexa.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Services
{
    public class ShopBookingReadService : IShopBookingReadService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IShopBookingReadRepository _repository;

        public ShopBookingReadService(
            IShopRepository shopRepository,
            IShopBookingReadRepository repository)
        {
            _shopRepository = shopRepository;
            _repository = repository;
        }

        public async Task<IEnumerable<ShopBookingListItemDto>> GetShopBookingsAsync(Guid shopOwnerId)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(shopOwnerId);
            if (shop == null)
                throw new Exception("Shop not found");

            return await _repository.GetShopBookingsAsync(shop.Id);
        }

        public async Task<bool> UpdateBookingStatusAsync(Guid shopOwnerId, UpdateBookingStatusDto dto)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(shopOwnerId);
            if (shop == null)
                throw new Exception("Shop not found");

            return await _repository.UpdateBookingStatusAsync(dto.BookingId, dto.Status);
        }
    }
}
