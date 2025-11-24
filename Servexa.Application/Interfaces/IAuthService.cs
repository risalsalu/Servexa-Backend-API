using Servexa.Application.DTOs.Auth;
using Servexa.Application.DTOs.Auth.Common;
using Servexa.Application.DTOs.Auth.Customer;
using Servexa.Application.DTOs.Auth.ShopOwner;

namespace Servexa.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(CustomerRegisterDto dto);
        Task<AuthResponseDto> RegisterShopOwnerAsync(ShopOwnerRegisterDto dto);

        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(Guid userId);

        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);

        Task<AuthResponseDto> SocialLoginAsync(SocialLoginDto dto);

        Task<UserProfileDto> GetCurrentUserAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    }
}
