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
        private readonly IShopRepository _shopRepository;
        private readonly IShopServiceRepository _shopServiceRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            ISlotRepository slotRepository,
            IShopRepository shopRepository,
            IShopServiceRepository shopServiceRepository)
        {
            _bookingRepository = bookingRepository;
            _slotRepository = slotRepository;
            _shopRepository = shopRepository;
            _shopServiceRepository = shopServiceRepository;
        }

        public async Task<BookingResponseDto> CreateDraftAsync(Guid customerId, CreateBookingDto dto)
        {
            var services = await _shopServiceRepository.GetByIdsAsync(dto.ServiceIds);
            var total = services.Sum(x => x.Price);

            if (total <= 0)
                throw new Exception("Invalid booking amount");

            var booking = new Booking
            {
                CustomerId = customerId,
                ShopId = dto.ShopId,
                ServiceMode = dto.ServiceMode,
                Status = BookingStatus.Draft,
                TotalAmount = total
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
                TotalAmount = total,
                Status = booking.Status.ToString()
            };
        }

        public async Task<bool> SelectAddressAsync(Guid bookingId, Guid addressId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                return false;

            if (booking.ServiceMode != ServiceMode.Home)
                return false;

            if (booking.Status != BookingStatus.Draft)
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

            if (booking.ServiceMode != ServiceMode.Onsite)
                return false;

            if (booking.Status != BookingStatus.Draft)
                return false;

            if (!await _slotRepository.IsSlotAvailableAsync(slotId))
                return false;

            if (!await _slotRepository.LockSlotAsync(slotId, customerId))
                return false;

            booking.SlotId = slotId;
            booking.AddressId = null;

            return await _bookingRepository.UpdateAsync(booking);
        }

        public async Task<bool> CancelAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                return false;

            if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
                return false;

            var slotId = booking.SlotId;

            booking.Status = BookingStatus.Cancelled;
            booking.SlotId = null;

            var updated = await _bookingRepository.UpdateAsync(booking);

            if (updated && slotId.HasValue)
                await _slotRepository.DeleteAsync(slotId.Value);

            return updated;
        }

        public async Task<bool> UpdateStatusAsync(Guid bookingId, int newStatus, Guid shopOwnerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return false;

            var shop = await _shopRepository.GetByIdAsync(booking.ShopId);
            if (shop == null || shop.OwnerId != shopOwnerId)
                return false;

            var targetStatus = (BookingStatus)newStatus;

            if (booking.Status == BookingStatus.Confirmed && targetStatus == BookingStatus.Completed)
            {
                booking.Status = BookingStatus.Completed;
                return await _bookingRepository.UpdateAsync(booking);
            }

            return false;
        }

        public async Task<BookingDetailDto> GetSummaryAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.CustomerId != customerId)
                throw new Exception("Booking not found");

            var items = await _bookingRepository.GetItemsByBookingIdAsync(bookingId);

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
            return await Map(await _bookingRepository.GetByCustomerAsync(customerId));
        }

        public async Task<IEnumerable<BookingDetailDto>> GetByShopAsync(Guid shopOwnerId)
        {
            var shop = await _shopRepository.GetByOwnerIdAsync(shopOwnerId);
            if (shop == null)
                throw new Exception("Shop not found");

            return await Map(await _bookingRepository.GetByShopAsync(shop.Id));
        }

        private async Task<IEnumerable<BookingDetailDto>> Map(IEnumerable<Booking> bookings)
        {
            var list = new List<BookingDetailDto>();

            foreach (var booking in bookings)
            {
                var items = await _bookingRepository.GetItemsByBookingIdAsync(booking.Id);

                list.Add(new BookingDetailDto
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

            return list;
        }
    }
}
