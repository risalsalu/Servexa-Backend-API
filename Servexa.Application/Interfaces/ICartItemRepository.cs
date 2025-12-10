using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        Task<CartItem?> GetByCartAndServiceAsync(Guid cartId, Guid shopServiceId, DateTime selectedDateTime);
        Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(Guid cartId);
    }
}
