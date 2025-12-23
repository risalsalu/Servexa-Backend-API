using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Booking
{
    public class ShopBookingListItemDto
    {
        public Guid BookingId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public IEnumerable<BookingItemDto> Services { get; set; } = [];
    }
}
