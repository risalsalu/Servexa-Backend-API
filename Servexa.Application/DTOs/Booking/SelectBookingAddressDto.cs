using System;

namespace Servexa.Application.DTOs.Booking
{
    public class SelectBookingAddressDto
    {
        public Guid BookingId { get; set; }
        public Guid AddressId { get; set; }
    }
}
