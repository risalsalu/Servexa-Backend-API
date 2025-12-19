using System;

namespace Servexa.Application.DTOs.Checkout
{
    public class PaymentResponseDto
    {
        public Guid BookingId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
