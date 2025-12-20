using System;

namespace Servexa.Application.DTOs.Booking
{
    public class BookingItemDto
    {
        public Guid ServiceId { get; set; }
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
