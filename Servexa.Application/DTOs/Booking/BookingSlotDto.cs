using System;

namespace Servexa.Application.DTOs.Booking
{
    public class BookingSlotDto
    {
        public DateTime BookingDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
    }
}
