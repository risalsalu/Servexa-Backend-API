using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Auth;

public interface IJwtGenerator
{
    string GenerateToken(User user, out int expiresInSeconds);
}
