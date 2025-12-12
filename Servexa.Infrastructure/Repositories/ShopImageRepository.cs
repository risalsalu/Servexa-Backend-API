using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories
{
    public class ShopImageRepository : IShopImageRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ShopImageRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection Conn() => _connectionFactory.CreateConnection();

        public async Task<ShopImage> AddAsync(ShopImage image)
        {
            image.Id = Guid.NewGuid();
            image.CreatedOn = DateTime.UtcNow;

            const string sql = @"
INSERT INTO ShopImages
(Id, ShopId, ImageUrl, PublicId, ImageType, IsDeleted, CreatedOn)
VALUES
(@Id, @ShopId, @ImageUrl, @PublicId, @ImageType, 0, @CreatedOn)";

            using var db = Conn();
            await db.ExecuteAsync(sql, image);
            return image;
        }

        public async Task<List<ShopImage>> GetByShopIdAsync(Guid shopId)
        {
            const string sql = "SELECT * FROM ShopImages WHERE ShopId = @shopId AND IsDeleted = 0";
            using var db = Conn();
            return (await db.QueryAsync<ShopImage>(sql, new { shopId })).AsList();
        }

        public async Task<ShopImage?> GetByIdAsync(Guid id)
        {
            const string sql = "SELECT TOP 1 * FROM ShopImages WHERE Id = @id AND IsDeleted = 0";
            using var db = Conn();
            return await db.QueryFirstOrDefaultAsync<ShopImage>(sql, new { id });
        }

        public async Task DeleteAsync(Guid id)
        {
            const string sql = "UPDATE ShopImages SET IsDeleted = 1 WHERE Id = @id";
            using var db = Conn();
            await db.ExecuteAsync(sql, new { id });
        }

        public async Task<string?> GetPrimaryImageUrlAsync(Guid shopId)
        {
            const string sql = @"SELECT TOP 1 ImageUrl 
                                 FROM ShopImages 
                                 WHERE ShopId = @shopId AND IsDeleted = 0 
                                 ORDER BY CreatedOn DESC";
            using var db = Conn();
            return await db.ExecuteScalarAsync<string?>(sql, new { shopId });
        }

        public async Task UpdateExistingImageAsync(Guid shopId, string imageType, string imageUrl, string publicId)
        {
            const string getSql = @"SELECT TOP 1 * 
                                    FROM ShopImages 
                                    WHERE ShopId = @shopId AND ImageType = @imageType AND IsDeleted = 0";

            using var db = Conn();
            var existing = await db.QueryFirstOrDefaultAsync<ShopImage>(getSql, new { shopId, imageType });

            if (existing == null)
            {
                var newImage = new ShopImage
                {
                    Id = Guid.NewGuid(),
                    ShopId = shopId,
                    ImageUrl = imageUrl,
                    PublicId = publicId,
                    ImageType = imageType,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                const string insertSql = @"
INSERT INTO ShopImages
(Id, ShopId, ImageUrl, PublicId, ImageType, IsDeleted, CreatedOn)
VALUES
(@Id, @ShopId, @ImageUrl, @PublicId, @ImageType, 0, @CreatedOn)";

                await db.ExecuteAsync(insertSql, newImage);
                return;
            }

            const string updateSql = @"
UPDATE ShopImages
SET ImageUrl = @imageUrl,
    PublicId = @publicId
WHERE Id = @id";

            await db.ExecuteAsync(updateSql, new
            {
                id = existing.Id,
                imageUrl,
                publicId
            });
        }
    }
}
