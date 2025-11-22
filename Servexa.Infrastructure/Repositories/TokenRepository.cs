using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SaveRefreshTokenAsync(RefreshToken token)
    {
        const string sql = @"
            INSERT INTO RefreshTokens (UserId, Token, ExpiresAt, CreatedAt, IsRevoked)
            VALUES (@UserId, @Token, @ExpiresAt, @CreatedAt, @IsRevoked);";

        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(sql, token);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        const string sql = @"SELECT * FROM RefreshTokens WHERE Token = @token";
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { token });
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        const string sql = @"UPDATE RefreshTokens SET IsRevoked = 1 WHERE Token = @token";
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(sql, new { token });
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        const string sql = @"UPDATE RefreshTokens SET IsRevoked = 1 WHERE UserId = @userId";
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(sql, new { userId });
    }
}
