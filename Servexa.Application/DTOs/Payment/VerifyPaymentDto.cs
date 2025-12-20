namespace Servexa.Application.DTOs.Payment
{
    public class VerifyPaymentDto
    {
        public string RazorpayOrderId { get; set; } = null!;
        public string RazorpayPaymentId { get; set; } = null!;
        public string RazorpaySignature { get; set; } = null!;
    }
}
