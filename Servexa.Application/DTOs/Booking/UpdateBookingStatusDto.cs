using System;

namespace Servexa.Application.DTOs.Booking
{
    public class UpdateBookingStatusDto
    {
        public Guid BookingId { get; set; }
        public int Status { get; set; }
    }
}
