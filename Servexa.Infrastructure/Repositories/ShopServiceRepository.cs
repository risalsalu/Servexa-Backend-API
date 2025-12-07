using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class ShopServiceRepository : GenericRepository<ShopService>, IShopServiceRepository
    {
        private readonly IDbConnectionFactory _factory;

        public ShopServiceRepository(IDbConnectionFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<ShopService>> GetByShopAsync(Guid shopId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<ShopService>(
                "SELECT * FROM ShopServices WHERE ShopId = @shopId AND IsDeleted = 0",
                new { shopId });
        }

        public async Task<IEnumerable<ShopService>> GetActiveByShopAsync(Guid shopId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<ShopService>(
                "SELECT * FROM ShopServices WHERE ShopId = @shopId AND IsActive = 1 AND IsDeleted = 0",
                new { shopId });
        }
    }
}
