using System;
using Servexa.Domain.Models;

namespace Servexa.Application.DTOs.Booking
{
    public class CreateBookingFromCartDto
    {
        public Guid ShopId { get; set; }
        public Guid? AddressId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public ServiceMode ServiceMode { get; set; }
    }
}
