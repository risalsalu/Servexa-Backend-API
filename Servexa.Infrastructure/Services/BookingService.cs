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

        public async Task<BookingResponseDto> CreateAsync(CreateBookingDto dto, Guid customerId)
        {
            if (dto.ServiceMode == ServiceMode.Home)
            {
                if (!dto.AddressId.HasValue)
                    throw new Exception("Address is required for home service");

                var address = await _addressRepository.GetByIdAsync(dto.AddressId.Value);
                if (address == null || address.UserId != customerId || address.IsDeleted)
                    throw new Exception("Invalid address");
            }

            if (dto.ServiceMode == ServiceMode.Onsite)
            {
                if (!dto.SlotId.HasValue)
                    throw new Exception("Slot is required for onsite service");

                var available = await _slotRepository.IsSlotAvailableAsync(dto.SlotId.Value);
                if (!available)
                    throw new Exception("Selected slot is already booked");

                var locked = await _slotRepository.LockSlotAsync(dto.SlotId.Value, customerId);
                if (!locked)
                    throw new Exception("Failed to lock slot");
            }

            var cartResponse = await _cartService.GetCartForShopAsync(customerId, dto.ShopId);
            var cart = cartResponse.Data;

            if (cart == null || cart.Items == null || !cart.Items.Any())
                throw new Exception("Cart is empty");

            var totalAmount = cart.TotalPrice;

            var booking = new Booking
            {
                CustomerId = customerId,
                ShopId = dto.ShopId,
                ServiceMode = dto.ServiceMode,
                AddressId = dto.AddressId,
                SlotId = dto.SlotId,
                Amount = totalAmount,
                Status = "Booked"
            };

            var bookingId = await _bookingRepository.CreateAsync(booking);

            return new BookingResponseDto
            {
                BookingId = bookingId,
                Amount = totalAmount,
                Status = "Booked"
            };
        }

        public async Task<IEnumerable<BookingDetailDto>> GetByCustomerAsync(Guid customerId)
        {
            var bookings = await _bookingRepository.GetByCustomerAsync(customerId);

            return bookings.Select(b => new BookingDetailDto
            {
                BookingId = b.Id,
                ShopId = b.ShopId,
                ServiceMode = b.ServiceMode,
                AddressId = b.AddressId,
                SlotId = b.SlotId,
                Amount = b.Amount,
                Status = b.Status,
                CreatedOn = b.CreatedOn
            });
        }
    }
}
