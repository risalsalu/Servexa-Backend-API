using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface ISlotRepository
    {
        Task<bool> HasOverlapAsync(Guid shopId, DateTime start, DateTime end);
        Task<bool> IsSlotAvailableAsync(Guid slotId);
        Task<bool> LockSlotAsync(Guid slotId, Guid customerId);
        Task<bool> ReleaseAsync(Guid slotId);
        Task<bool> DeleteAsync(Guid slotId);
        Task AddAsync(Slot slot);
        Task<IEnumerable<Slot>> GetAvailableSlotsAsync(Guid shopId, DateTime date);
        Task<Slot?> GetByIdAsync(Guid slotId);
    }
}
