namespace Servexa.Application.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public string OrderId { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = null!;
    }
}
