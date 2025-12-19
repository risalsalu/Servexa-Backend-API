using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface ISlotRepository
    {
        Task<bool> IsSlotAvailableAsync(Guid slotId);
        Task<bool> LockSlotAsync(Guid slotId, Guid customerId);
        Task<bool> SlotExistsAsync(Guid shopId, DateTime start, DateTime end);
        Task<bool> SlotsExistForDateAsync(Guid shopId, DateTime date);
        Task AddAsync(Slot slot);
        Task<IEnumerable<Slot>> GetAvailableSlotsAsync(Guid shopId, DateTime date);
    }
}
