using Dapper;
using Servexa.Application.DTOs.Admin;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByEmailOrPhoneAsync(string value)
        {
            const string sql = @"SELECT * FROM Users WHERE (Email = @value OR Phone = @value) AND IsDeleted = 0";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { value });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            const string sql = @"SELECT * FROM Users WHERE Email = @email AND IsDeleted = 0";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { email });
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            const string sql = @"SELECT * FROM Users WHERE Id = @id AND IsDeleted = 0";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { id });
        }

        public async Task<bool> EmailOrPhoneExistsAsync(string email, string phone)
        {
            const string sql = @"SELECT COUNT(1) FROM Users WHERE (Email = @email OR Phone = @phone) AND IsDeleted = 0";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { email, phone }) > 0;
        }

        public async Task CreateAsync(User user)
        {
            const string sql = @"
INSERT INTO Users (
    Id, FullName, Email, PasswordHash, Role, Phone, IsActive, BusinessName,
    CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, DeletedBy, DeletedOn, IsDeleted,
    ProfileImageUrl, ProfileImagePublicId, Gender, DateOfBirth, Address, Bio
)
VALUES (
    @Id, @FullName, @Email, @PasswordHash, @Role, @Phone, @IsActive, @BusinessName,
    @CreatedBy, @CreatedOn, @ModifiedBy, @ModifiedOn, @DeletedBy, @DeletedOn, @IsDeleted,
    @ProfileImageUrl, @ProfileImagePublicId, @Gender, @DateOfBirth, @Address, @Bio
)";
            using var conn = _connectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, user);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            const string sql = @"
UPDATE Users SET
    FullName = @FullName,
    Email = @Email,
    PasswordHash = @PasswordHash,
    Role = @Role,
    Phone = @Phone,
    BusinessName = @BusinessName,
    Gender = @Gender,
    DateOfBirth = @DateOfBirth,
    Address = @Address,
    Bio = @Bio,
    ModifiedBy = @ModifiedBy,
    ModifiedOn = @ModifiedOn,
    IsActive = @IsActive,
    IsDeleted = @IsDeleted,
    ProfileImageUrl = @ProfileImageUrl,
    ProfileImagePublicId = @ProfileImagePublicId
WHERE Id = @Id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, user) > 0;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string sql = @"SELECT * FROM Users WHERE IsDeleted = 0";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<User>(sql);
        }

        public async Task<bool> SetActiveStatusAsync(Guid id, bool isActive)
        {
            const string sql = @"UPDATE Users SET IsActive = @isActive WHERE Id = @id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, new { id, isActive }) > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, Guid deletedBy)
        {
            const string sql = @"
UPDATE Users
SET IsDeleted = 1,
    DeletedBy = @deletedBy,
    DeletedOn = @now
WHERE Id = @id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, new { id, deletedBy, now = DateTime.UtcNow }) > 0;
        }

        public async Task<IEnumerable<AdminShopOwnerListDto>> GetAllShopOwnersWithShopStatusAsync()
        {
            const string sql = @"
SELECT
    u.Id,
    u.FullName,
    u.Email,
    u.Phone,
    u.IsActive,
    s.Id AS ShopId,
    s.ShopName,
    s.IsActive AS ShopIsActive,
    s.OfflineReason AS ShopOfflineReason
FROM Users u
LEFT JOIN Shops s ON s.OwnerId = u.Id AND s.IsDeleted = 0
WHERE u.Role = 'ShopOwner' AND u.IsDeleted = 0
";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<AdminShopOwnerListDto>(sql);
        }
    }
}
