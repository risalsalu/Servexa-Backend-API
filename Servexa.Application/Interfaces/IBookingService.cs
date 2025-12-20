using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;

namespace Servexa.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAfterPaymentAsync(
            Guid customerId,
            CreateBookingAfterPaymentDto dto
        );

        Task<IEnumerable<BookingDetailDto>> GetByCustomerAsync(Guid customerId);

        Task<IEnumerable<BookingDetailDto>> GetByShopAsync(Guid shopOwnerId);

        Task<BookingDetailDto> GetByIdAsync(Guid bookingId);

        Task<bool> CancelAsync(Guid bookingId, Guid customerId);

        Task<bool> UpdateStatusAsync(Guid bookingId, string status, Guid shopOwnerId);
    }
}
