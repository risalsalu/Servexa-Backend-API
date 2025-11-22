using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone)
    {
        const string sql = @"SELECT * FROM Users
                             WHERE Email = @emailOrPhone OR Phone = @emailOrPhone";

        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { emailOrPhone });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = @"SELECT * FROM Users WHERE Email = @email";
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { email });
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT * FROM Users WHERE UserId = @id";
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { id });
    }

    public async Task<bool> EmailOrPhoneExistsAsync(string email, string phone)
    {
        const string sql = @"SELECT COUNT(1) FROM Users
                             WHERE Email = @email OR Phone = @phone";
        using var conn = _connectionFactory.CreateConnection();
        var count = await conn.ExecuteScalarAsync<int>(sql, new { email, phone });
        return count > 0;
    }

    public async Task CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO Users (UserId, FullName, Email, PasswordHash, Role, Phone, CreatedAt, IsActive)
            VALUES (@UserId, @FullName, @Email, @PasswordHash, @Role, @Phone, @CreatedAt, @IsActive);";

        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(sql, user);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE Users
               SET FullName = @FullName,
                   Email    = @Email,
                   PasswordHash = @PasswordHash,
                   Role     = @Role,
                   Phone    = @Phone,
                   IsActive = @IsActive
             WHERE UserId = @UserId";

        using var conn = _connectionFactory.CreateConnection();
        var rows = await conn.ExecuteAsync(sql, user);
        return rows > 0;
    }
}
