using System;

namespace Servexa.Application.DTOs.Booking
{
    public class BookingItemDto
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
    }
}
