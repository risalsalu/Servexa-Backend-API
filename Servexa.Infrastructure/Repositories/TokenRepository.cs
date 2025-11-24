using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly IDbConnectionFactory _factory;

    public TokenRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task SaveRefreshTokenAsync(RefreshToken token)
    {
        const string sql = @"
            INSERT INTO RefreshTokens 
            (Id, UserId, Token, ExpiresAt, CreatedOn, IsRevoked, IsDeleted)
            VALUES
            (@Id, @UserId, @Token, @ExpiresAt, @CreatedOn, @IsRevoked, 0)";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, token);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        const string sql = "SELECT * FROM RefreshTokens WHERE Token = @token AND IsDeleted = 0";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { token });
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        const string sql = "UPDATE RefreshTokens SET IsRevoked = 1 WHERE Token = @token";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { token });
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        const string sql = "UPDATE RefreshTokens SET IsRevoked = 1 WHERE UserId = @userId";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { userId });
    }
}
