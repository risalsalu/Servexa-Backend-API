using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Payment;
using Servexa.Application.DTOs.Booking;

namespace Servexa.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateOrderAsync(
            CreatePaymentOrderDto dto,
            Guid customerId
        );

        Task<BookingResponseDto> VerifyPaymentAsync(
            VerifyPaymentDto dto,
            Guid customerId
        );
    }
}
