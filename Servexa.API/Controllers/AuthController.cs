using Microsoft.AspNetCore.Mvc;
using Servexa.API.Controllers;
using Servexa.Application.DTOs.Auth;
using Servexa.Application.DTOs.Auth.Common;
using Servexa.Application.DTOs.Auth.Customer;
using Servexa.Application.DTOs.Auth.ShopOwner;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] CustomerRegisterDto dto)
        {
            var result = await _authService.RegisterUserAsync(dto);
            return Success(result, "User registration successful");
        }

        [HttpPost("register-shopowner")]
        public async Task<IActionResult> RegisterShopOwner([FromBody] ShopOwnerRegisterDto dto)
        {
            var result = await _authService.RegisterShopOwnerAsync(dto);
            return Success(result, "Shop owner registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Success(result, "Login successful");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return Success(result, "Token refreshed");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
        {
            await _authService.LogoutAsync(dto.UserId);
            return SuccessMessage("Logged out");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return SuccessMessage("Reset instructions sent");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return SuccessMessage("Password reset successful");
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> SocialLogin([FromBody] SocialLoginDto dto)
        {
            var result = await _authService.SocialLoginAsync(dto);
            return Success(result, "Social login successful");
        }
    }
}
