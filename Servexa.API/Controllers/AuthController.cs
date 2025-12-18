using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Auth;
using Servexa.Application.DTOs.Auth.Common;
using Servexa.Application.DTOs.Auth.Customer;
using Servexa.Application.DTOs.Auth.ShopOwner;
using Servexa.Application.Interfaces;
using System;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
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
            return Created(result, "User registration successful");
        }

        [HttpPost("register-shopowner")]
        public async Task<IActionResult> RegisterShopOwner([FromBody] ShopOwnerRegisterDto dto)
        {
            var result = await _authService.RegisterShopOwnerAsync(dto);
            return Created(result, "Shop owner registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            Response.Cookies.Append("access_token", result.Token, BuildAccessCookie(result.ExpiresIn));
            Response.Cookies.Append("refresh_token", result.RefreshToken, BuildRefreshCookie());

            return Success(new
            {
                result.Role,
                result.UserId
            }, "Login successful");
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> SocialLogin([FromBody] SocialLoginDto dto)
        {
            var result = await _authService.SocialLoginAsync(dto);

            Response.Cookies.Append("access_token", result.Token, BuildAccessCookie(result.ExpiresIn));
            Response.Cookies.Append("refresh_token", result.RefreshToken, BuildRefreshCookie());

            return Success(new
            {
                result.Role,
                result.UserId
            }, "Social login successful");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return UnauthorizedError("Refresh token missing");

            var result = await _authService.RefreshTokenAsync(refreshToken);

            Response.Cookies.Append("access_token", result.Token, BuildAccessCookie(result.ExpiresIn));
            Response.Cookies.Append("refresh_token", result.RefreshToken, BuildRefreshCookie());

            return Success(new
            {
                result.Role,
                result.UserId
            }, "Token refreshed");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
        {
            await _authService.LogoutAsync(dto.UserId);

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return SuccessMessage("Logged out successfully");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var token = await _authService.ForgotPasswordAsync(dto);
            return Success(token, "Reset instructions sent");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return SuccessMessage("Password reset successful");
        }

        private static CookieOptions BuildAccessCookie(int expiresIn)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddSeconds(expiresIn)
            };
        }

        private static CookieOptions BuildRefreshCookie()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
        }
    }
}
