using System;
using Servexa.Domain.Models;

namespace Servexa.Application.DTOs.Booking
{
    public class CreateBookingDto
    {
        public Guid ShopId { get; set; }
        public ServiceMode ServiceMode { get; set; }
        public Guid? AddressId { get; set; }
        public Guid? SlotId { get; set; }
    }
}
