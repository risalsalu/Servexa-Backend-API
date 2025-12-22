using System;

namespace Servexa.Application.DTOs.Booking
{
    public class SelectBookingSlotDto
    {
        public Guid BookingId { get; set; }
        public Guid SlotId { get; set; }
    }
}
