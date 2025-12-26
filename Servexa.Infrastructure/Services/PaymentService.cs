using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using Servexa.Application.DTOs.Payment;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Settings;

namespace Servexa.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly RazorpaySettings _settings;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IOptions<RazorpaySettings> options)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _settings = options.Value;
        }

        public async Task<PaymentResponseDto> CreateOrderAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                throw new Exception("Invalid booking");

            if (booking.TotalAmount < 1)
                throw new Exception("Invalid booking amount");

            if (booking.Status != BookingStatus.Draft &&
                booking.Status != BookingStatus.PendingPayment &&
                booking.Status != BookingStatus.PaymentFailed)
                throw new Exception("Booking not eligible for payment");

            return await CreateNewPaymentAsync(booking, customerId);
        }

        public async Task<PaymentResponseDto> RetryPaymentAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                throw new Exception("Invalid booking");

            if (booking.Status != BookingStatus.PendingPayment &&
                booking.Status != BookingStatus.PaymentFailed)
                throw new Exception("Retry not allowed");

            var lastPayment = await _paymentRepository.GetLatestByBookingIdAsync(bookingId);
            if (lastPayment != null && lastPayment.Status == PaymentStatus.Created)
            {
                lastPayment.Status = PaymentStatus.Failed;
                await _paymentRepository.UpdateAsync(lastPayment);
            }

            return await CreateNewPaymentAsync(booking, customerId);
        }

        private async Task<PaymentResponseDto> CreateNewPaymentAsync(Booking booking, Guid customerId)
        {
            var amountInPaise = (int)(booking.TotalAmount * 100);
            if (amountInPaise < 100)
                throw new Exception("Order amount less than minimum allowed");

            var client = new RazorpayClient(_settings.KeyId, _settings.KeySecret);

            var order = client.Order.Create(new System.Collections.Generic.Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", "INR" },
                { "receipt", $"SX_{DateTime.UtcNow.Ticks}" }
            });

            var payment = new Servexa.Domain.Models.Payment
            {
                BookingId = booking.Id,
                UserId = customerId,
                ShopId = booking.ShopId,
                Amount = booking.TotalAmount,
                RazorpayOrderId = order["id"].ToString(),
                Status = PaymentStatus.Created
            };

            await _paymentRepository.CreateAsync(payment);

            booking.Status = BookingStatus.PendingPayment;
            await _bookingRepository.UpdateAsync(booking);

            return new PaymentResponseDto
            {
                OrderId = payment.RazorpayOrderId,
                KeyId = _settings.KeyId,
                Amount = payment.Amount,
                PaymentStatus = payment.Status.ToString()
            };
        }

        public async Task<bool> VerifyPaymentAsync(VerifyPaymentDto dto, Guid customerId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(dto.RazorpayOrderId);
            if (payment == null || payment.Status != PaymentStatus.Created)
                throw new Exception("Invalid payment");

            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
            if (booking == null || booking.CustomerId != customerId)
                throw new Exception("Invalid booking");

            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.KeySecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            if (signature != dto.RazorpaySignature)
                throw new Exception("Invalid signature");

            payment.RazorpayPaymentId = dto.RazorpayPaymentId;
            payment.RazorpaySignature = dto.RazorpaySignature;
            payment.Status = PaymentStatus.Paid;

            await _paymentRepository.UpdateAsync(payment);

            booking.Status = BookingStatus.Confirmed;
            await _bookingRepository.UpdateAsync(booking);

            return true;
        }
    }
}
