using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using System.Data;

namespace Servexa.Infrastructure.Repositories
{
    public class CustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CustomerAddressRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<CustomerAddress>> GetByUserIdAsync(Guid userId)
        {
            const string sql = @"
                SELECT *
                FROM CustomerAddresses
                WHERE UserId = @userId AND IsDeleted = 0
                ORDER BY CreatedOn DESC";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<CustomerAddress>(sql, new { userId });
        }

        public async Task<CustomerAddress?> GetByIdAsync(Guid id)
        {
            const string sql = @"
                SELECT *
                FROM CustomerAddresses
                WHERE Id = @id AND IsDeleted = 0";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<CustomerAddress>(sql, new { id });
        }

        public async Task<Guid> AddAsync(CustomerAddress address)
        {
            const string sql = @"
                INSERT INTO CustomerAddresses (
                    Id,
                    UserId,
                    Label,
                    Line1,
                    City,
                    Pincode,
                    Lat,
                    Lng,
                    CreatedBy,
                    CreatedOn,
                    ModifiedBy,
                    ModifiedOn,
                    DeletedBy,
                    DeletedOn,
                    IsDeleted
                )
                VALUES (
                    @Id,
                    @UserId,
                    @Label,
                    @Line1,
                    @City,
                    @Pincode,
                    @Lat,
                    @Lng,
                    @CreatedBy,
                    @CreatedOn,
                    @ModifiedBy,
                    @ModifiedOn,
                    @DeletedBy,
                    @DeletedOn,
                    @IsDeleted
                )";

            using var conn = _connectionFactory.CreateConnection();
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

            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, address);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid deletedBy)
        {
            const string sql = @"
                UPDATE CustomerAddresses
                SET IsDeleted = 1,
                    DeletedBy = @deletedBy,
                    DeletedOn = @now
                WHERE Id = @id AND IsDeleted = 0";

            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, new
            {
                id,
                deletedBy,
                now = DateTime.UtcNow
            });

            return rows > 0;
        }
    }
}
