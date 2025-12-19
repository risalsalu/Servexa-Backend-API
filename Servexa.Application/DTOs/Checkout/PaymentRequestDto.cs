using System;

namespace Servexa.Application.DTOs.Checkout
{
    public class PaymentRequestDto
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
