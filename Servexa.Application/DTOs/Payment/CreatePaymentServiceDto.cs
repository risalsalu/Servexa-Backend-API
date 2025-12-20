using System;

namespace Servexa.Application.DTOs.Payment
{
    public class CreatePaymentServiceDto
    {
        public Guid ServiceId { get; set; }
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
