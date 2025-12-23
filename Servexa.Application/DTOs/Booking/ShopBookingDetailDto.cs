using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Booking
{
    public class ShopBookingDetailDto
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public Guid ShopId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public IEnumerable<BookingItemDto> Services { get; set; } = [];
    }
}
