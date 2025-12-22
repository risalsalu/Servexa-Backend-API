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
        private readonly ICustomerAddressRepository _addressRepository;
        private readonly IShopServiceRepository _shopServiceRepository;
        private readonly IShopRepository _shopRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            ISlotRepository slotRepository,
            ICustomerAddressRepository addressRepository,
            IShopServiceRepository shopServiceRepository,
            IShopRepository shopRepository)
        {
            _bookingRepository = bookingRepository;
            _slotRepository = slotRepository;
            _addressRepository = addressRepository;
            _shopServiceRepository = shopServiceRepository;
            _shopRepository = shopRepository;
        }

        public async Task<BookingResponseDto> CreateDraftAsync(Guid customerId, CreateBookingDto dto)
        {
            var shopActive = await _shopRepository.IsShopActiveAsync(dto.ShopId);
            if (!shopActive)
                throw new Exception("This shop is currently offline and cannot accept bookings");

            var existingDraft = (await _bookingRepository.GetByCustomerAsync(customerId))
                .Any(b => b.ShopId == dto.ShopId && b.Status == BookingStatus.Draft);

            if (existingDraft)
                throw new Exception("Draft booking already exists");

            var services = await _shopServiceRepository.GetByIdsAsync(dto.ServiceIds);
            if (!services.Any())
                throw new Exception("Invalid services");

            var totalAmount = services.Sum(s => s.Price);

            var booking = new Booking
            {
                CustomerId = customerId,
                ShopId = dto.ShopId,
                ServiceMode = dto.ServiceMode,
                Status = BookingStatus.Draft,
                TotalAmount = totalAmount
            };

            var bookingId = await _bookingRepository.CreateAsync(booking);

            var items = services.Select(s => new BookingItem
            {
                BookingId = bookingId,
                ServiceId = s.Id,
                Price = s.Price,
                DurationInMinutes = s.DurationMinutes
            });

            await _bookingRepository.AddItemsAsync(items);

            return new BookingResponseDto
            {
                BookingId = bookingId,
                TotalAmount = totalAmount,
                Status = booking.Status.ToString()
            };
        }

        public async Task<bool> SelectAddressAsync(Guid bookingId, Guid addressId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                return false;

            if (booking.Status != BookingStatus.Draft)
                return false;

            if (booking.ServiceMode != ServiceMode.Home)
                throw new Exception("This booking is not a home service");

            if (booking.AddressId != null)
                return false;

            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null || address.UserId != customerId || address.IsDeleted)
                return false;

            booking.AddressId = addressId;
            booking.SlotId = null;

            return await _bookingRepository.UpdateAsync(booking);
        }

        public async Task<bool> SelectSlotAsync(Guid bookingId, Guid slotId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                return false;

            if (booking.Status != BookingStatus.Draft)
                return false;

            if (booking.ServiceMode != ServiceMode.Onsite)
                throw new Exception("This booking is not an onsite service");

            if (booking.SlotId != null)
                return false;

            var available = await _slotRepository.IsSlotAvailableAsync(slotId);
            if (!available)
                return false;

            var locked = await _slotRepository.LockSlotAsync(slotId, customerId);
            if (!locked)
                return false;

            booking.SlotId = slotId;
            booking.AddressId = null;

            return await _bookingRepository.UpdateAsync(booking);
        }

        public async Task<BookingDetailDto> GetSummaryAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                throw new Exception("Booking not found");

            var items = await _bookingRepository.GetItemsByBookingIdAsync(booking.Id);

            return new BookingDetailDto
            {
                BookingId = booking.Id,
                ShopId = booking.ShopId,
                ServiceMode = booking.ServiceMode.ToString(),
                AddressId = booking.AddressId,
                SlotId = booking.SlotId,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status.ToString(),
                CreatedOn = booking.CreatedOn,
                Services = items.Select(i => new BookingItemDto
                {
                    ServiceId = i.ServiceId,
                    Price = i.Price,
                    DurationInMinutes = i.DurationInMinutes
                })
            };
        }

        public async Task<IEnumerable<BookingDetailDto>> GetByCustomerAsync(Guid customerId)
        {
            var bookings = await _bookingRepository.GetByCustomerAsync(customerId);
            var result = new List<BookingDetailDto>();

            foreach (var booking in bookings)
            {
                var items = await _bookingRepository.GetItemsByBookingIdAsync(booking.Id);

                result.Add(new BookingDetailDto
                {
                    BookingId = booking.Id,
                    ShopId = booking.ShopId,
                    ServiceMode = booking.ServiceMode.ToString(),
                    AddressId = booking.AddressId,
                    SlotId = booking.SlotId,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status.ToString(),
                    CreatedOn = booking.CreatedOn,
                    Services = items.Select(i => new BookingItemDto
                    {
                        ServiceId = i.ServiceId,
                        Price = i.Price,
                        DurationInMinutes = i.DurationInMinutes
                    })
                });
            }

            return result;
        }

        public async Task<IEnumerable<BookingDetailDto>> GetByShopAsync(Guid shopOwnerId)
        {
            var bookings = await _bookingRepository.GetByShopAsync(shopOwnerId);
            var result = new List<BookingDetailDto>();

            foreach (var booking in bookings)
            {
                var items = await _bookingRepository.GetItemsByBookingIdAsync(booking.Id);

                result.Add(new BookingDetailDto
                {
                    BookingId = booking.Id,
                    ShopId = booking.ShopId,
                    ServiceMode = booking.ServiceMode.ToString(),
                    AddressId = booking.AddressId,
                    SlotId = booking.SlotId,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status.ToString(),
                    CreatedOn = booking.CreatedOn,
                    Services = items.Select(i => new BookingItemDto
                    {
                        ServiceId = i.ServiceId,
                        Price = i.Price,
                        DurationInMinutes = i.DurationInMinutes
                    })
                });
            }

            return result;
        }

        public async Task<bool> CancelAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                return false;

            if (booking.Status != BookingStatus.Draft &&
                booking.Status != BookingStatus.PendingPayment)
                return false;

            booking.Status = BookingStatus.Cancelled;
            return await _bookingRepository.UpdateAsync(booking);
        }
    }
}
