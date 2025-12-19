using System;
using Servexa.Domain.Models;

namespace Servexa.Application.DTOs.Booking
{
    public class BookingDetailDto
    {
        public Guid BookingId { get; set; }
        public Guid ShopId { get; set; }
        public ServiceMode ServiceMode { get; set; }
        public Guid? AddressId { get; set; }
        public Guid? SlotId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
    }
}
