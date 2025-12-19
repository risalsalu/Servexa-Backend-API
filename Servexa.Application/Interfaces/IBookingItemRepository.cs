using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;

namespace Servexa.Application.Interfaces
{
    public interface IBookingItemRepository
    {
        Task AddAsync(Guid bookingId, BookingItemDto dto, Guid createdBy);
    }
}
