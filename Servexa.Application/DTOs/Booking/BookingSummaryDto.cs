using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Booking
{
    public class BookingSummaryDto
    {
        public Guid BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceMode { get; set; } = string.Empty;
        public IEnumerable<BookingItemDto> Items { get; set; } = [];
    }
}
