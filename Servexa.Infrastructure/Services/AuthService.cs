using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servexa.Application.DTOs.Auth.Common;
using Servexa.Application.DTOs.Auth.Customer;
using Servexa.Application.DTOs.Auth.ShopOwner;
using Servexa.Application.DTOs.Users;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenRepository _tokenRepo;
        private readonly IConfiguration _config;
        private readonly ICloudinaryService _cloudinary;
        private readonly HttpClient _http;

        public AuthService(
            IUserRepository userRepo,
            ITokenRepository tokenRepo,
            IConfiguration config,
            ICloudinaryService cloudinary,
            IHttpClientFactory factory)
        {
            _userRepo = userRepo;
            _tokenRepo = tokenRepo;
            _config = config;
            _cloudinary = cloudinary;
            _http = factory.CreateClient();
        }

        public async Task<AuthResponseDto> RegisterUserAsync(CustomerRegisterDto dto)
        {
            if (await _userRepo.EmailOrPhoneExistsAsync(dto.Email, dto.Phone))
                throw new ApplicationException("Email or phone already in use");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Customer",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            await _userRepo.CreateAsync(user);
            return await GenerateTokensForUser(user);
        }

        public async Task<AuthResponseDto> RegisterShopOwnerAsync(ShopOwnerRegisterDto dto)
        {
            if (await _userRepo.EmailOrPhoneExistsAsync(dto.Email, dto.Phone))
                throw new ApplicationException("Email or phone already in use");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.OwnerName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "ShopOwner",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                BusinessName = dto.BusinessName,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            await _userRepo.CreateAsync(user);
            return await GenerateTokensForUser(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailOrPhoneAsync(dto.EmailOrPhone)
                ?? throw new ApplicationException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new ApplicationException("Invalid credentials");

            if (!user.IsActive)
                throw new ApplicationException("Account disabled");

            return await GenerateTokensForUser(user);
        }

        public async Task<AuthResponseDto> SocialLoginAsync(SocialLoginDto dto)
        {
            if (dto.Provider != "google")
                throw new ApplicationException("Unsupported provider");

            return await GoogleLoginInternal(dto.IdToken);
        }

        public async Task<AuthResponseDto> GoogleLoginAsync(string idToken)
        {
            return await GoogleLoginInternal(idToken);
        }

        private async Task<AuthResponseDto> GoogleLoginInternal(string idToken)
        {
            var googleUser = await VerifyGoogleTokenAsync(idToken);

            var user = await _userRepo.GetByEmailAsync(googleUser.Email);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = googleUser.FullName,
                    Email = googleUser.Email,
                    Phone = string.Empty,
                    Role = "Customer",
                    PasswordHash = string.Empty,
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
                ?? throw new ApplicationException("Invalid refresh token");

            if (stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
                throw new ApplicationException("Refresh token expired");

            var user = await _userRepo.GetByIdAsync(stored.UserId)
                ?? throw new ApplicationException("User not found");

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
            return user?.Id.ToString();
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (!Guid.TryParse(dto.Token, out var userId))
                throw new ApplicationException("Invalid token");

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _userRepo.UpdateAsync(user);
        }

        public async Task<UserProfileDto> GetCurrentUserAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found");

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                ProfileImageUrl = user.ProfileImageUrl,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Bio = user.Bio,
                BusinessName = user.BusinessName
            };
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.Address = dto.Address;
            user.Bio = dto.Bio;
            user.BusinessName = dto.BusinessName;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _userRepo.UpdateAsync(user);
        }

        public async Task UpdateContactInfoAsync(Guid userId, UpdateContactInfoDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _userRepo.UpdateAsync(user);
        }

        public async Task<string?> UploadProfileImageAsync(Guid userId, IFormFile file)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found");

            if (!string.IsNullOrEmpty(user.ProfileImagePublicId))
                await _cloudinary.DeleteAsync(user.ProfileImagePublicId);

            var (url, publicId) = await _cloudinary.UploadAsync(file);

            user.ProfileImageUrl = url;
            user.ProfileImagePublicId = publicId;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _userRepo.UpdateAsync(user);
            return url;
        }

        public async Task DeleteProfileImageAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found");

            if (!string.IsNullOrEmpty(user.ProfileImagePublicId))
                await _cloudinary.DeleteAsync(user.ProfileImagePublicId);

            user.ProfileImageUrl = null;
            user.ProfileImagePublicId = null;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _userRepo.UpdateAsync(user);
        }

        private async Task<AuthResponseDto> GenerateTokensForUser(User user)
        {
            var accessToken = GenerateJwtToken(user, out var expiresIn);
            var refreshToken = await GenerateAndStoreRefreshToken(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn,
                Role = user.Role,
                UserId = user.Id
            };
        }

        private string GenerateJwtToken(User user, out int expiresInSeconds)
        {
            var jwt = _config.GetSection("Jwt");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Role, user.Role),
                new(JwtRegisteredClaimNames.Email, user.Email)
            };

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwt["AccessTokenMinutes"]!));

            var token = new JwtSecurityToken(
                jwt["Issuer"],
                jwt["Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            expiresInSeconds = (int)(expires - DateTime.UtcNow).TotalSeconds;
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateAndStoreRefreshToken(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);

            var token = Convert.ToBase64String(bytes);

            await _tokenRepo.SaveRefreshTokenAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = token,
                CreatedOn = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(jwt["RefreshTokenDays"]!)),
                IsRevoked = false,
                IsDeleted = false
            });

            return token;
        }

        private async Task<GoogleUserInfoDto> VerifyGoogleTokenAsync(string idToken)
        {
            var res = await _http.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");

            if (!res.IsSuccessStatusCode)
                throw new ApplicationException("Invalid Google token");

            var json = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json).RootElement;

            return new GoogleUserInfoDto
            {
                Email = doc.GetProperty("email").GetString()!,
                FullName = doc.GetProperty("name").GetString()!,
                ProviderUserId = doc.GetProperty("sub").GetString()!
            };
        }
    }
}
