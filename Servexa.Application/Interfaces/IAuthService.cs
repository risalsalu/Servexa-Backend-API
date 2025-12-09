using Microsoft.AspNetCore.Http;
using Servexa.Application.DTOs.Auth;
using Servexa.Application.DTOs.Auth.Common;
using Servexa.Application.DTOs.Auth.Customer;
using Servexa.Application.DTOs.Auth.ShopOwner;
using Servexa.Application.DTOs.Users;

namespace Servexa.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(CustomerRegisterDto dto);
        Task<AuthResponseDto> RegisterShopOwnerAsync(ShopOwnerRegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> SocialLoginAsync(SocialLoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(Guid userId);
        Task<string?> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task<UserProfileDto> GetCurrentUserAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        Task UpdateContactInfoAsync(Guid userId, UpdateContactInfoDto dto);
        Task<string?> UploadProfileImageAsync(Guid userId, IFormFile file);
        Task DeleteProfileImageAsync(Guid userId);
    }
}
