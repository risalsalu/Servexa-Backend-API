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

        public SlotService(ISlotRepository slotRepository)
        {
            _slotRepository = slotRepository;
        }

        public async Task<Guid> CreateSlotAsync(CreateSlotDto dto, Guid customerId)
        {
            var start = dto.Date.Date + dto.StartTime;
            var end = dto.Date.Date + dto.EndTime;

            if (start.TimeOfDay < TimeSpan.FromHours(9) || end.TimeOfDay > TimeSpan.FromHours(18))
                throw new Exception("Slot must be between 09:00 and 18:00");

            var duration = (end - start).TotalMinutes;
            if (duration < 15 || duration > 30)
                throw new Exception("Slot duration must be between 15 and 30 minutes");

            var overlap = await _slotRepository.HasOverlapAsync(dto.ShopId, start, end);
            if (overlap)
                throw new Exception("Slot already booked or overlapping");

            var slot = new Slot
            {
                ShopId = dto.ShopId,
                StartTime = start,
                EndTime = end
            };

            await _slotRepository.AddAsync(slot);
            return slot.Id;
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

        public async Task<bool> BookSlotAsync(Guid slotId, Guid customerId)
        {
            var available = await _slotRepository.IsSlotAvailableAsync(slotId);
            if (!available)
                throw new Exception("Slot not available");

            return await _slotRepository.MarkBookedAsync(slotId, customerId);
        }
    }
}
