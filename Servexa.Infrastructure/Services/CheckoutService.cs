using System;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Checkout;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IBookingRepository _bookingRepository;

        public CheckoutService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<PaymentResponseDto> PayAsync(InitiateCheckoutDto dto, Guid customerId)
        {
            await _bookingRepository.UpdateStatusAsync(dto.BookingId, BookingStatus.Confirmed, customerId);

            return new PaymentResponseDto
            {
                BookingId = dto.BookingId,
                PaymentStatus = "SUCCESS"
            };
        }
    }
}
