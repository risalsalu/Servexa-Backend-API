using System;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetActiveCartForUserAndShopAsync(Guid userId, Guid shopId);
    }
}
