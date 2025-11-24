using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces;

public interface ITokenRepository
{
    Task SaveRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);

    Task RevokeRefreshTokenAsync(string token);
    Task RevokeAllForUserAsync(Guid userId);
}
