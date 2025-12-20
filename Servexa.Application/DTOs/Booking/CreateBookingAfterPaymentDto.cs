using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Booking
{
    public class CreateBookingAfterPaymentDto
    {
        public Guid ShopId { get; set; }
        public string ServiceMode { get; set; } = null!;
        public Guid? AddressId { get; set; }
        public Guid? SlotId { get; set; }
        public decimal Amount { get; set; }
        public IEnumerable<BookingItemDto> Services { get; set; } = [];
        public Guid PaymentId { get; set; }
    }
}
