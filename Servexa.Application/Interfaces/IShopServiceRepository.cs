using Servexa.Application.DTOs.Services;
using Servexa.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces
{
    public interface IShopServiceRepository : IGenericRepository<ShopService>
    {
        Task<IEnumerable<ShopService>> GetByShopAsync(Guid shopId);
        Task<IEnumerable<ShopService>> GetActiveByShopAsync(Guid shopId);
        Task<ShopServiceDetailsDto?> GetServiceWithDetailsAsync(Guid serviceId);
        Task<IEnumerable<ShopService>> GetByIdsAsync(IEnumerable<Guid> serviceIds);
    }
}
