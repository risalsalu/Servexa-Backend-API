using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ShopRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection Conn() => _connectionFactory.CreateConnection();

        public async Task<bool> OwnerHasShopAsync(Guid ownerId)
        {
            const string sql = "SELECT COUNT(1) FROM Shops WHERE OwnerId = @ownerId AND IsDeleted = 0";
            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(sql, new { ownerId }) > 0;
        }

        public async Task<Guid> CreateAsync(Shop shop)
        {
            shop.Id = Guid.NewGuid();
            shop.CreatedOn = DateTime.UtcNow;

            const string sql = @"
INSERT INTO Shops
(Id, OwnerId, ShopName, CategoryId, Description, Address, Latitude, Longitude, Phone,
 HomeServiceAvailable, WorkingHours, IsActive, OfflineReason, IsDeleted, CreatedOn)
VALUES
(@Id, @OwnerId, @ShopName, @CategoryId, @Description, @Address, @Latitude, @Longitude, @Phone,
 @HomeServiceAvailable, @WorkingHours, @IsActive, @OfflineReason, 0, @CreatedOn)";

            using var db = Conn();
            await db.ExecuteAsync(sql, shop);
            return shop.Id;
        }

        public async Task<Shop?> GetByOwnerIdAsync(Guid ownerId)
        {
            const string sql = "SELECT * FROM Shops WHERE OwnerId = @ownerId AND IsDeleted = 0";
            using var db = Conn();
            return await db.QueryFirstOrDefaultAsync<Shop>(sql, new { ownerId });
        }

        public async Task<Shop?> GetByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM Shops WHERE Id = @id AND IsDeleted = 0";
            using var db = Conn();
            return await db.QueryFirstOrDefaultAsync<Shop>(sql, new { id });
        }

        public async Task UpdateAsync(Shop shop)
        {
            shop.ModifiedOn = DateTime.UtcNow;

            const string sql = @"
UPDATE Shops SET
ShopName = @ShopName,
CategoryId = @CategoryId,
Description = @Description,
Address = @Address,
Latitude = @Latitude,
Longitude = @Longitude,
Phone = @Phone,
HomeServiceAvailable = @HomeServiceAvailable,
WorkingHours = @WorkingHours,
ModifiedOn = @ModifiedOn
WHERE Id = @Id AND IsDeleted = 0";

            using var db = Conn();
            await db.ExecuteAsync(sql, shop);
        }

        public async Task SetActiveStatusAsync(Guid ownerId, bool isActive, string? offlineReason)
        {
            const string sql = @"
UPDATE Shops
SET IsActive = @isActive,
    OfflineReason = @offlineReason,
    ModifiedOn = @modifiedOn
WHERE OwnerId = @ownerId AND IsDeleted = 0";

            using var db = Conn();
            await db.ExecuteAsync(sql, new
            {
                ownerId,
                isActive,
                offlineReason = isActive ? null : offlineReason,
                modifiedOn = DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<Shop>> GetActiveShopsAsync()
        {
            const string sql = "SELECT * FROM Shops WHERE IsDeleted = 0 AND IsActive = 1";
            using var db = Conn();
            return await db.QueryAsync<Shop>(sql);
        }

        public async Task<IEnumerable<Shop>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Shops WHERE IsDeleted = 0";
            using var db = Conn();
            return await db.QueryAsync<Shop>(sql);
        }

        public async Task<bool> IsShopActiveAsync(Guid shopId)
        {
            const string sql = "SELECT IsActive FROM Shops WHERE Id = @shopId AND IsDeleted = 0";
            using var db = Conn();
            return await db.ExecuteScalarAsync<bool>(sql, new { shopId });
        }

        public async Task<IEnumerable<Shop>> GetNearbyShopsAsync(decimal customerLat, decimal customerLng, int radiusKm)
        {
            const string sql = @"
SELECT *
FROM Shops
WHERE IsDeleted = 0
AND Latitude IS NOT NULL
AND Longitude IS NOT NULL
AND (
    6371 * ACOS(
        COS(RADIANS(@customerLat)) *
        COS(RADIANS(Latitude)) *
        COS(RADIANS(Longitude) - RADIANS(@customerLng)) +
        SIN(RADIANS(@customerLat)) *
        SIN(RADIANS(Latitude))
    )
) <= @radiusKm";

            using var db = Conn();
            return await db.QueryAsync<Shop>(sql, new
            {
                customerLat,
                customerLng,
                radiusKm
            });
        }
    }
}
