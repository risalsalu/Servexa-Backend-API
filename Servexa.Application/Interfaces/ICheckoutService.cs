using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Checkout;

namespace Servexa.Application.Interfaces
{
    public interface ICheckoutService
    {
        Task<PaymentResponseDto> PayAsync(InitiateCheckoutDto dto, Guid customerId);
    }
}
