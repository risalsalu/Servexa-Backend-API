using System;

namespace Servexa.Application.DTOs.Checkout
{
    public class CheckoutSummaryDto
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }
}
