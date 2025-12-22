using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;

namespace Servexa.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateDraftAsync(Guid customerId, CreateBookingDto dto);

        Task<bool> SelectAddressAsync(Guid bookingId, Guid addressId, Guid customerId);

        Task<bool> SelectSlotAsync(Guid bookingId, Guid slotId, Guid customerId);

        Task<BookingDetailDto> GetSummaryAsync(Guid bookingId, Guid customerId);

        Task<IEnumerable<BookingDetailDto>> GetByCustomerAsync(Guid customerId);

        Task<IEnumerable<BookingDetailDto>> GetByShopAsync(Guid shopOwnerId);

        Task<bool> CancelAsync(Guid bookingId, Guid customerId);
    }
}
