using Dapper;
using Servexa.Application.DTOs.Services;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<ShopService>> GetByIdsAsync(IEnumerable<Guid> serviceIds)
        {
            using var conn = _factory.CreateConnection();

            const string sql = @"
            SELECT *
            FROM ShopServices
            WHERE Id IN @serviceIds AND IsDeleted = 0";

            return await conn.QueryAsync<ShopService>(sql, new { serviceIds });
        }


        public async Task<ShopServiceDetailsDto?> GetServiceWithDetailsAsync(Guid serviceId)
        {
            using var conn = _factory.CreateConnection();

            var sql = @"
                SELECT 
                    ss.Id,
                    ss.Name AS ServiceName,
                    ss.Price,
                    ss.DurationMinutes,
                    ss.ShopId,
                    s.ShopName,
                    c.Name AS CategoryName
                FROM ShopServices ss
                INNER JOIN Shops s ON s.Id = ss.ShopId
                INNER JOIN Categories c ON c.Id = ss.CategoryId
                WHERE ss.Id = @serviceId AND ss.IsDeleted = 0";

            return await conn.QueryFirstOrDefaultAsync<ShopServiceDetailsDto>(sql, new { serviceId });
        }
    }
}
