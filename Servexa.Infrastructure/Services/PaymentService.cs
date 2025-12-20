using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RazorpayClientAlias = Razorpay.Api.RazorpayClient;
using Servexa.Application.DTOs.Booking;
using Servexa.Application.DTOs.Payment;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Settings;

namespace Servexa.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingService _bookingService;
        private readonly RazorpaySettings _settings;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingService bookingService,
            IOptions<RazorpaySettings> options)
        {
            _paymentRepository = paymentRepository;
            _bookingService = bookingService;
            _settings = options.Value;
        }

        public async Task<PaymentResponseDto> CreateOrderAsync(
            CreatePaymentOrderDto dto,
            Guid customerId)
        {
            var client = new RazorpayClientAlias(_settings.KeyId, _settings.KeySecret);

            var order = client.Order.Create(new System.Collections.Generic.Dictionary<string, object>
            {
                { "amount", (int)(dto.Amount * 100) },
                { "currency", "INR" },
                { "receipt", $"SX_{DateTime.UtcNow.Ticks}" }
            });

            var payment = new Servexa.Domain.Models.Payment
            {
                UserId = customerId,
                ShopId = dto.ShopId,
                Amount = dto.Amount,
                RazorpayOrderId = order["id"].ToString(),
                Status = PaymentStatus.Created,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.CreateAsync(payment);

            return new PaymentResponseDto
            {
                OrderId = payment.RazorpayOrderId,
                KeyId = _settings.KeyId,
                Amount = dto.Amount,
                PaymentStatus = payment.Status.ToString()
            };
        }

        public async Task<BookingResponseDto> VerifyPaymentAsync(
            VerifyPaymentDto dto,
            Guid customerId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(dto.RazorpayOrderId);
            if (payment == null || payment.Status != PaymentStatus.Created)
                throw new Exception("Invalid payment");

            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.KeySecret));
            var computed = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

            if (computed != dto.RazorpaySignature)
                throw new Exception("Invalid signature");

            payment.RazorpayPaymentId = dto.RazorpayPaymentId;
            payment.RazorpaySignature = dto.RazorpaySignature;
            payment.Status = PaymentStatus.Paid;

            await _paymentRepository.UpdateAsync(payment);

            return await _bookingService.CreateBookingAfterPaymentAsync(
                customerId,
                new CreateBookingAfterPaymentDto
                {
                    ShopId = payment.ShopId,
                    Amount = payment.Amount,
                    PaymentId = payment.Id
                });
        }
    }
}
