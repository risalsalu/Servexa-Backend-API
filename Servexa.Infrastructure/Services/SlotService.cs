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
                throw new Exception("Invalid slot time");

            var duration = (end - start).TotalMinutes;
            if (duration < 15 || duration > 30)
                throw new Exception("Invalid slot duration");

            if (await _slotRepository.HasOverlapAsync(dto.ShopId, start, end))
                throw new Exception("Slot already exists");

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
    }
}
