using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Slot;

namespace Servexa.Application.Interfaces
{
    public interface ISlotService
    {
        Task<int> CreateSlotsAsync(CreateSlotDto dto, Guid ownerId);
        Task<IEnumerable<SlotResponseDto>> GetAvailableSlotsAsync(Guid shopId, DateTime date);
    }
}
