using System;
using System.Linq;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly ICartService _cartService;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingItemRepository _bookingItemRepository;

        public BookingService(
            ICartService cartService,
            IBookingRepository bookingRepository,
            IBookingItemRepository bookingItemRepository)
        {
            _cartService = cartService;
            _bookingRepository = bookingRepository;
            _bookingItemRepository = bookingItemRepository;
        }

        public async Task<BookingResponseDto> CreateAsync(Guid customerId, CreateBookingFromCartDto dto)
        {
            if (dto.ServiceMode == ServiceMode.Home && dto.AddressId == null)
                throw new Exception("Address required");

            var cartResponse = await _cartService.GetCartForShopAsync(customerId, dto.ShopId);

            if (cartResponse.Data == null || !cartResponse.Data.Items.Any())
                throw new Exception("Cart is empty");

            var cart = cartResponse.Data;

            var booking = new Booking
            {
                UserId = customerId,
                ShopId = cart.ShopId,
                AddressId = dto.ServiceMode == ServiceMode.Home ? dto.AddressId : null,
                BookingDate = dto.BookingDate,
                SlotStart = dto.SlotStart,
                SlotEnd = dto.SlotEnd,
                TotalAmount = cart.Items.Sum(x => x.Price * x.Quantity),
                Status = BookingStatus.Pending,
                ServiceMode = dto.ServiceMode,
                CreatedBy = customerId
            };

            await _bookingRepository.AddAsync(booking);

            foreach (var item in cart.Items)
            {
                await _bookingItemRepository.AddAsync(
                    booking.Id,
                    new BookingItemDto
                    {
                        ServiceId = item.ShopServiceId,
                        ServiceName = item.ServiceName,
                        Price = item.Price,
                        DurationMinutes = item.DurationMinutes
                    },
                    customerId
                );
            }

            await _cartService.ClearCartAsync(customerId, cart.ShopId);

            return new BookingResponseDto
            {
                BookingId = booking.Id,
                Status = booking.Status.ToString()
            };
        }

        public async Task<BookingSummaryDto> GetSummaryAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            return new BookingSummaryDto
            {
                BookingId = booking.Id,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status.ToString(),
                ServiceMode = booking.ServiceMode.ToString()
            };
        }

        public async Task UpdateStatusAsync(UpdateBookingStatusDto dto, Guid updatedBy)
        {
            var status = Enum.Parse<BookingStatus>(dto.Status, true);
            await _bookingRepository.UpdateStatusAsync(dto.BookingId, status, updatedBy);
        }
    }
}
