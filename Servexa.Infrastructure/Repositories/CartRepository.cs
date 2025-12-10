using System;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly IDbConnectionFactory _factory;

        public CartRepository(IDbConnectionFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public async Task<Cart?> GetActiveCartForUserAndShopAsync(Guid userId, Guid shopId)
        {
            using var connection = _factory.CreateConnection();
            var sql = @"SELECT TOP 1 * FROM Carts 
                        WHERE UserId = @UserId AND ShopId = @ShopId AND IsDeleted = 0
                        ORDER BY CreatedOn DESC";
            return await connection.QueryFirstOrDefaultAsync<Cart>(sql, new { UserId = userId, ShopId = shopId });
        }
    }
}
