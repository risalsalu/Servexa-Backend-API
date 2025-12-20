using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Booking;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISlotRepository _slotRepository;
        private readonly ICartService _cartService;
        private readonly ICustomerAddressRepository _addressRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            ISlotRepository slotRepository,
            ICartService cartService,
            ICustomerAddressRepository addressRepository)
        {
            _bookingRepository = bookingRepository;
            _slotRepository = slotRepository;
            _cartService = cartService;
            _addressRepository = addressRepository;
        }

        public async Task<BookingResponseDto> CreateBookingAfterPaymentAsync(
            Guid customerId,
            CreateBookingAfterPaymentDto dto)
        {
            var serviceMode = Enum.Parse<ServiceMode>(dto.ServiceMode, true);

            if (serviceMode == ServiceMode.Home)
            {
                if (!dto.AddressId.HasValue)
                    throw new Exception("Address is required");

                var address = await _addressRepository.GetByIdAsync(dto.AddressId.Value);
                if (address == null || address.UserId != customerId || address.IsDeleted)
                    throw new Exception("Invalid address");
            }

            if (serviceMode == ServiceMode.Onsite)
            {
                if (!dto.SlotId.HasValue)
                    throw new Exception("Slot is required");

                var available = await _slotRepository.IsSlotAvailableAsync(dto.SlotId.Value);
                if (!available)
                    throw new Exception("Slot already booked");

                var locked = await _slotRepository.LockSlotAsync(dto.SlotId.Value, customerId);
                if (!locked)
                    throw new Exception("Slot lock failed");
            }

            var cartResponse = await _cartService.GetCartForShopAsync(customerId, dto.ShopId);
            var cart = cartResponse.Data;

            if (cart == null || cart.Items == null || !cart.Items.Any())
                throw new Exception("Cart is empty");

            var booking = new Booking
            {
                CustomerId = customerId,
                ShopId = dto.ShopId,
                PaymentId = dto.PaymentId,
                ServiceMode = serviceMode,
                AddressId = dto.AddressId,
                SlotId = dto.SlotId,
                Amount = dto.Amount,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            var bookingId = await _bookingRepository.CreateAsync(booking);

            var items = dto.Services.Select(s => new BookingItem
            {
                BookingId = bookingId,
                ServiceId = s.ServiceId,
                Price = s.Price,
                DurationInMinutes = s.DurationInMinutes
            });

            await _bookingRepository.AddItemsAsync(items);

            await _cartService.ClearCartAsync(customerId, dto.ShopId);

            return new BookingResponseDto
            {
                BookingId = bookingId,
                Amount = booking.Amount,
                Status = booking.Status.ToString()
            };
        }

        public async Task<IEnumerable<BookingDetailDto>> GetByCustomerAsync(Guid customerId)
        {
            var bookings = await _bookingRepository.GetByCustomerAsync(customerId);

            return bookings.Select(b => new BookingDetailDto
            {
                BookingId = b.Id,
                ShopId = b.ShopId,
                ServiceMode = b.ServiceMode.ToString(),
                AddressId = b.AddressId,
                SlotId = b.SlotId,
                Amount = b.Amount,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            });
        }

        public async Task<IEnumerable<BookingDetailDto>> GetByShopAsync(Guid shopOwnerId)
        {
            var bookings = await _bookingRepository.GetByShopAsync(shopOwnerId);

            return bookings.Select(b => new BookingDetailDto
            {
                BookingId = b.Id,
                ShopId = b.ShopId,
                ServiceMode = b.ServiceMode.ToString(),
                AddressId = b.AddressId,
                SlotId = b.SlotId,
                Amount = b.Amount,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            });
        }

        public async Task<BookingDetailDto> GetByIdAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                throw new Exception("Booking not found");

            return new BookingDetailDto
            {
                BookingId = booking.Id,
                ShopId = booking.ShopId,
                ServiceMode = booking.ServiceMode.ToString(),
                AddressId = booking.AddressId,
                SlotId = booking.SlotId,
                Amount = booking.Amount,
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt
            };
        }

        public async Task<bool> CancelAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                return false;

            if (booking.Status == BookingStatus.Completed)
                return false;

            return await _bookingRepository.UpdateStatusAsync(bookingId, BookingStatus.Cancelled);
        }

        public async Task<bool> UpdateStatusAsync(Guid bookingId, string status, Guid shopOwnerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return false;

            var parsedStatus = Enum.Parse<BookingStatus>(status, true);
            return await _bookingRepository.UpdateStatusAsync(bookingId, parsedStatus);
        }
    }
}
