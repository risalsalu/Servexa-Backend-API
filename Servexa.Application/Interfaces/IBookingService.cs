using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;

namespace Servexa.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateAsync(Guid customerId, CreateBookingFromCartDto dto);
        Task<BookingSummaryDto> GetSummaryAsync(Guid bookingId);
        Task UpdateStatusAsync(UpdateBookingStatusDto dto, Guid updatedBy);
    }
}
