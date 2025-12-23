using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;

namespace Servexa.Application.Interfaces
{
    public interface IShopBookingReadRepository
    {
        Task<IEnumerable<ShopBookingListItemDto>> GetShopBookingsAsync(Guid shopId);
        Task<bool> UpdateBookingStatusAsync(Guid bookingId, int status);
    }
}
