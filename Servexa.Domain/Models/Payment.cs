using System;

namespace Servexa.Domain.Models
{
    public class Payment : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ShopId { get; set; }
        public decimal Amount { get; set; }
        public string RazorpayOrderId { get; set; } = null!;
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
