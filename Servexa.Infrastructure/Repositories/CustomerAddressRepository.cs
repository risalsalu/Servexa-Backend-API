using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Repositories
{
    public class CustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CustomerAddressRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection Conn() => _connectionFactory.CreateConnection();

        public async Task<IEnumerable<CustomerAddress>> GetByUserIdAsync(Guid userId)
        {
            const string sql = @"
SELECT *
FROM CustomerAddresses
WHERE UserId = @userId AND IsDeleted = 0
ORDER BY CreatedOn DESC";

            using var conn = Conn();
            return await conn.QueryAsync<CustomerAddress>(sql, new { userId });
        }

        public async Task<CustomerAddress?> GetByIdAsync(Guid id)
        {
            const string sql = @"
SELECT *
FROM CustomerAddresses
WHERE Id = @id AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.QueryFirstOrDefaultAsync<CustomerAddress>(sql, new { id });
        }

        public async Task<CustomerAddress?> GetActiveAddressAsync(Guid userId)
        {
            const string sql = @"
SELECT TOP 1 *
FROM CustomerAddresses
WHERE UserId = @userId AND IsDeleted = 0
ORDER BY CreatedOn DESC";

            using var conn = Conn();
            return await conn.QueryFirstOrDefaultAsync<CustomerAddress>(sql, new { userId });
        }

        public async Task<Guid> AddAsync(CustomerAddress address)
        {
            const string sql = @"
INSERT INTO CustomerAddresses
(Id, UserId, Label, Line1, City, Pincode, Lat, Lng, CreatedBy, CreatedOn, IsDeleted)
VALUES
(@Id, @UserId, @Label, @Line1, @City, @Pincode, @Lat, @Lng, @CreatedBy, @CreatedOn, 0)";

            using var conn = Conn();
            await conn.ExecuteAsync(sql, address);
            return address.Id;
        }

        public async Task<bool> UpdateAsync(CustomerAddress address)
        {
            const string sql = @"
UPDATE CustomerAddresses
SET Label = @Label,
    Line1 = @Line1,
    City = @City,
    Pincode = @Pincode,
    Lat = @Lat,
    Lng = @Lng,
    ModifiedBy = @ModifiedBy,
    ModifiedOn = @ModifiedOn
WHERE Id = @Id AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.ExecuteAsync(sql, address) > 0;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid deletedBy)
        {
            const string sql = @"
UPDATE CustomerAddresses
SET IsDeleted = 1,
    DeletedBy = @deletedBy,
    DeletedOn = @now
WHERE Id = @id AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.ExecuteAsync(sql, new
            {
                id,
                deletedBy,
                now = DateTime.UtcNow
            }) > 0;
        }
    }
}
