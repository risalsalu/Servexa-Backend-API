using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        private readonly IDbConnectionFactory _factory;

        public CartItemRepository(IDbConnectionFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public async Task<CartItem?> GetByCartAndServiceAsync(Guid cartId, Guid shopServiceId, DateTime selectedDateTime)
        {
            using var connection = _factory.CreateConnection();
            var sql = @"SELECT TOP 1 * FROM CartItems
                        WHERE CartId = @CartId 
                          AND ShopServiceId = @ShopServiceId 
                          AND SelectedDateTime = @SelectedDateTime 
                          AND IsDeleted = 0";
            return await connection.QueryFirstOrDefaultAsync<CartItem>(sql, new { CartId = cartId, ShopServiceId = shopServiceId, SelectedDateTime = selectedDateTime });
        }

        public async Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(Guid cartId)
        {
            using var connection = _factory.CreateConnection();
            var sql = @"SELECT * FROM CartItems
                        WHERE CartId = @CartId 
                          AND IsDeleted = 0";
            return await connection.QueryAsync<CartItem>(sql, new { CartId = cartId });
        }
    }
}
