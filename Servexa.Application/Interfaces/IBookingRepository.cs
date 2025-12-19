using System;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task UpdateStatusAsync(Guid bookingId, BookingStatus status, Guid updatedBy);
    }
}
