using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servexa.Application.DTOs.Auth;
using Servexa.Application.DTOs.Auth.Common;
using Servexa.Application.DTOs.Auth.Customer;
using Servexa.Application.DTOs.Auth.ShopOwner;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services
{
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

        public async Task<AuthResponseDto> RegisterUserAsync(CustomerRegisterDto dto)
        {
            if (await _userRepo.EmailOrPhoneExistsAsync(dto.Email, dto.Phone))
                throw new ApplicationException("Email or phone already in use.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            await _userRepo.CreateAsync(user);

            return new AuthResponseDto
            {
                Token = "",
                RefreshToken = "",
                ExpiresIn = 0,
                Role = user.Role,
                UserId = user.Id
            };
        }

        public async Task<AuthResponseDto> RegisterShopOwnerAsync(ShopOwnerRegisterDto dto)
        {
            if (await _userRepo.EmailOrPhoneExistsAsync(dto.Email, dto.Phone))
                throw new ApplicationException("Email or phone already in use.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.OwnerName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                BusinessName = dto.BusinessName,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            await _userRepo.CreateAsync(user);

            return new AuthResponseDto
            {
                Token = "",
                RefreshToken = "",
                ExpiresIn = 0,
                Role = user.Role,
                UserId = user.Id
            };
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

        public async Task<AuthResponseDto> SocialLoginAsync(SocialLoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone ?? "",
                    Role = dto.Role,
                    PasswordHash = "",
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };

                await _userRepo.CreateAsync(user);
            }

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

        public async Task<string?> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepo.GetByEmailOrPhoneAsync(dto.EmailOrPhone);
            if (user == null)
                return null;

            return user.Id.ToString();
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (!Guid.TryParse(dto.Token, out var userId))
                throw new ApplicationException("Invalid reset token.");

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _userRepo.UpdateAsync(user);
        }

        public async Task<UserProfileDto> GetCurrentUserAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found.");

            return new UserProfileDto
            {
                Id = user.Id,
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
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _userRepo.UpdateAsync(user);
        }

        private async Task<AuthResponseDto> GenerateTokensForUser(User user)
        {
            var token = GenerateJwtToken(user, out var expires);
            var refresh = await GenerateAndStoreRefreshToken(user);

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refresh,
                ExpiresIn = expires,
                Role = user.Role,
                UserId = user.Id
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
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Role, user.Role),
                new(JwtRegisteredClaimNames.Email, user.Email)
            };

            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

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
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = tokenString,
                CreatedOn = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(days),
                IsRevoked = false,
                IsDeleted = false
            };

            await _tokenRepo.SaveRefreshTokenAsync(refresh);
            return tokenString;
        }
    }
}
