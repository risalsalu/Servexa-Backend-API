using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Booking
{
    public class BookingDetailsDto
    {
        public Guid BookingId { get; set; }
        public Guid ShopId { get; set; }
        public Guid UserId { get; set; }
        public Guid? AddressId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceMode { get; set; } = string.Empty;
        public IEnumerable<BookingItemDto> Items { get; set; } = [];
    }
}
