using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Payment;

namespace Servexa.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateOrderAsync(Guid bookingId, Guid customerId);

        Task<bool> VerifyPaymentAsync(VerifyPaymentDto dto, Guid customerId);
    }
}
