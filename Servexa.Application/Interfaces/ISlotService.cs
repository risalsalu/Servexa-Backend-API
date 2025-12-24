using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Application.DTOs.Slot;

namespace Servexa.Application.Interfaces
{
    public interface ISlotService
    {
        Task<Guid> CreateSlotAsync(CreateSlotDto dto, Guid customerId);
        Task<IEnumerable<SlotResponseDto>> GetAvailableSlotsAsync(Guid shopId, DateTime date);
        Task<bool> BookSlotAsync(Guid slotId, Guid customerId);
    }
}
