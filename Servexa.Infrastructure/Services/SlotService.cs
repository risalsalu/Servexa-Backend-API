using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Slot;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services
{
    public class SlotService : ISlotService
    {
        private readonly ISlotRepository _slotRepository;
        private readonly IShopRepository _shopRepository;

        public SlotService(
            ISlotRepository slotRepository,
            IShopRepository shopRepository)
        {
            _slotRepository = slotRepository;
            _shopRepository = shopRepository;
        }

        public async Task<int> CreateSlotsAsync(CreateSlotDto dto, Guid ownerId)
        {
            var shop = await _shopRepository.GetByIdAsync(dto.ShopId);
            if (shop == null || shop.OwnerId != ownerId)
                throw new Exception("Unauthorized shop access");

            var alreadyExists = await _slotRepository.SlotsExistForDateAsync(dto.ShopId, dto.Date);
            if (alreadyExists)
                throw new Exception("Slots already exist for this date");

            var created = 0;
            var current = dto.Date.Date + dto.StartTime;
            var end = dto.Date.Date + dto.EndTime;

            while (current.AddMinutes(30) <= end)
            {
                var slotEnd = current.AddMinutes(30);

                await _slotRepository.AddAsync(new Slot
                {
                    ShopId = dto.ShopId,
                    StartTime = current,
                    EndTime = slotEnd
                });

                created++;
                current = slotEnd;
            }

            return created;
        }

        public async Task<IEnumerable<SlotResponseDto>> GetAvailableSlotsAsync(Guid shopId, DateTime date)
        {
            var slots = await _slotRepository.GetAvailableSlotsAsync(shopId, date);

            return slots.Select(s => new SlotResponseDto
            {
                SlotId = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            });
        }

        public async Task<bool> DeleteSlotAsync(Guid slotId, Guid ownerId)
        {
            var slot = await _slotRepository.GetByIdAsync(slotId);
            if (slot == null)
                throw new Exception("Slot not found");

            if (slot.IsBooked)
                throw new Exception("Booked slot cannot be deleted");

            var shop = await _shopRepository.GetByIdAsync(slot.ShopId);
            if (shop == null || shop.OwnerId != ownerId)
                throw new Exception("Unauthorized access");

            return await _slotRepository.DeleteAsync(slotId);
        }
    }
}
