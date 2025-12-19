using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;

namespace Servexa.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateAsync(CreateBookingDto dto, Guid customerId);
        Task<IEnumerable<BookingDetailDto>> GetByCustomerAsync(Guid customerId);
    }
}
