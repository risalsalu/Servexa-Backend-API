using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories;

public class ShopImageRepository : IShopImageRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ShopImageRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private IDbConnection CreateConnection() => _connectionFactory.CreateConnection();

    public async Task<ShopImage> AddAsync(ShopImage image)
    {
        const string sql = @"
INSERT INTO ShopImages
(Id, ShopId, ImageUrl)
VALUES
(@Id, @ShopId, @ImageUrl)";
        image.Id = Guid.NewGuid();
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, image);
        return image;
    }

    public async Task<ShopImage?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT TOP 1 * FROM ShopImages WHERE Id = @Id";
        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ShopImage>(sql, new { Id = id });
    }

    public async Task<IEnumerable<ShopImage>> GetByShopIdAsync(Guid shopId)
    {
        const string sql = "SELECT * FROM ShopImages WHERE ShopId = @ShopId";
        using var connection = CreateConnection();
        return await connection.QueryAsync<ShopImage>(sql, new { ShopId = shopId });
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM ShopImages WHERE Id = @Id";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}
