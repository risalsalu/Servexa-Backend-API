using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servexa.Application.DTOs.Auth;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenRepository _tokenRepo;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepo, ITokenRepository tokenRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepo.EmailOrPhoneExistsAsync(dto.Email, dto.Phone))
            throw new ApplicationException("Email or phone already in use.");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Role = dto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepo.CreateAsync(user);
        return await GenerateTokensForUser(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetByEmailOrPhoneAsync(dto.EmailOrPhone)
                   ?? throw new ApplicationException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new ApplicationException("Invalid credentials.");

        if (!user.IsActive)
            throw new ApplicationException("Account is disabled.");

        return await GenerateTokensForUser(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var stored = await _tokenRepo.GetByTokenAsync(refreshToken)
                     ?? throw new ApplicationException("Invalid refresh token.");

        if (stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            throw new ApplicationException("Refresh token expired or revoked.");

        var user = await _userRepo.GetByIdAsync(stored.UserId)
                   ?? throw new ApplicationException("User not found.");

        await _tokenRepo.RevokeRefreshTokenAsync(refreshToken);
        return await GenerateTokensForUser(user);
    }

    public async Task LogoutAsync(Guid userId)
    {
        await _tokenRepo.RevokeAllForUserAsync(userId);
    }

    public Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        return Task.CompletedTask;
    }

    public Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        return Task.CompletedTask;
    }

    public Task<AuthResponseDto> SocialLoginAsync(SocialLoginDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<UserProfileDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId)
                   ?? throw new ApplicationException("User not found.");

        return new UserProfileDto
        {
            Id = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role
        };
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId)
                   ?? throw new ApplicationException("User not found.");

        user.FullName = dto.FullName;
        user.Phone = dto.Phone;

        await _userRepo.UpdateAsync(user);
    }

    private async Task<AuthResponseDto> GenerateTokensForUser(User user)
    {
        var accessToken = GenerateJwtToken(user, out var expiresInSeconds);
        var refreshToken = await GenerateAndStoreRefreshToken(user);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresInSeconds,
            Role = user.Role,
            UserId = user.UserId
        };
    }

    private string GenerateJwtToken(User user, out int expiresInSeconds)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection["Key"]!;
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var minutes = int.Parse(jwtSection["AccessTokenMinutes"] ?? "15");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        var expires = DateTime.UtcNow.AddMinutes(minutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        expiresInSeconds = (int)(expires - DateTime.UtcNow).TotalSeconds;
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateAndStoreRefreshToken(User user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var days = int.Parse(jwtSection["RefreshTokenDays"] ?? "7");

        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var tokenString = Convert.ToBase64String(randomBytes);

        var refresh = new RefreshToken
        {
            UserId = user.UserId,
            Token = tokenString,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(days),
            IsRevoked = false
        };

        await _tokenRepo.SaveRefreshTokenAsync(refresh);
        return tokenString;
    }
}
