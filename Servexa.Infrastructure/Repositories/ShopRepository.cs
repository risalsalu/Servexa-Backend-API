using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories;

public class ShopRepository : IShopRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ShopRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private IDbConnection CreateConnection() => _connectionFactory.CreateConnection();

    public async Task<bool> OwnerHasShopAsync(Guid ownerId)
    {
        const string sql = "SELECT COUNT(1) FROM Shops WHERE OwnerId = @OwnerId";
        using var connection = CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { OwnerId = ownerId });
        return count > 0;
    }

    public async Task<Guid> CreateAsync(Shop shop)
    {
        const string sql = @"
INSERT INTO Shops
(Id, OwnerId, ShopName, Categories, Description, Address, Latitude, Longitude, Phone,
 HomeServiceAvailable, LicenseImageUrl, IdProofImageUrl, IsActive, Services, WorkingHours)
VALUES
(@Id, @OwnerId, @ShopName, @Categories, @Description, @Address, @Latitude, @Longitude, @Phone,
 @HomeServiceAvailable, @LicenseImageUrl, @IdProofImageUrl, @IsActive, @Services, @WorkingHours)";
        shop.Id = Guid.NewGuid();
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, shop);
        return shop.Id;
    }

    public async Task<Shop?> GetByOwnerIdAsync(Guid ownerId)
    {
        const string sql = "SELECT TOP 1 * FROM Shops WHERE OwnerId = @OwnerId";
        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Shop>(sql, new { OwnerId = ownerId });
    }

    public async Task UpdateAsync(Shop shop)
    {
        const string sql = @"
UPDATE Shops SET
ShopName = @ShopName,
Categories = @Categories,
Description = @Description,
Address = @Address,
Latitude = @Latitude,
Longitude = @Longitude,
Phone = @Phone,
HomeServiceAvailable = @HomeServiceAvailable,
Services = @Services,
WorkingHours = @WorkingHours
WHERE OwnerId = @OwnerId";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, shop);
    }

    public async Task SetActiveStatusAsync(Guid ownerId, bool isActive)
    {
        const string sql = "UPDATE Shops SET IsActive = @IsActive WHERE OwnerId = @OwnerId";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new { OwnerId = ownerId, IsActive = isActive });
    }
}
