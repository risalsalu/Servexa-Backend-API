using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Booking
{
    public class BookingDetailDto
    {
        public Guid BookingId { get; set; }
        public Guid ShopId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string ServiceMode { get; set; } = null!;
        public Guid? AddressId { get; set; }
        public Guid? SlotId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public IEnumerable<BookingItemDto> Services { get; set; } = [];
        public DateTime CreatedOn { get; set; }
    }
}
