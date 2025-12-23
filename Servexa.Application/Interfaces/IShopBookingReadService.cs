using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;

namespace Servexa.Application.Interfaces
{
    public interface IShopBookingReadService
    {
        Task<IEnumerable<ShopBookingListItemDto>> GetShopBookingsAsync(Guid shopOwnerId);
        Task<bool> UpdateBookingStatusAsync(Guid shopOwnerId, UpdateBookingStatusDto dto);
    }
}
