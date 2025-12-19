using System;

namespace Servexa.Application.DTOs.Booking
{
    public class BookingResponseDto
    {
        public Guid BookingId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
