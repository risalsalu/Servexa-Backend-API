using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> RegisterUser(CustomerRegisterDto dto)
        {
            var result = await _authService.RegisterUserAsync(dto);
            return Created(result, "User registered");
        }

        [HttpPost("register-shopowner")]
        public async Task<IActionResult> RegisterShopOwner(ShopOwnerRegisterDto dto)
        {
            var result = await _authService.RegisterShopOwnerAsync(dto);
            return Created(result, "Shop owner registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            Response.Cookies.Append("access_token", result.AccessToken, BuildAccessCookie(result.ExpiresIn));
            Response.Cookies.Append("refresh_token", result.RefreshToken, BuildRefreshCookie());

            return Success(new { result.Role, result.UserId }, "Login successful");
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto dto)
        {
            var result = await _authService.GoogleLoginAsync(dto.IdToken);

            Response.Cookies.Append("access_token", result.AccessToken, BuildAccessCookie(result.ExpiresIn));
            Response.Cookies.Append("refresh_token", result.RefreshToken, BuildRefreshCookie());

            return Success(new { result.Role, result.UserId }, "Google login successful");
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> SocialLogin(SocialLoginDto dto)
        {
            var result = await _authService.SocialLoginAsync(dto);

            Response.Cookies.Append("access_token", result.AccessToken, BuildAccessCookie(result.ExpiresIn));
            Response.Cookies.Append("refresh_token", result.RefreshToken, BuildRefreshCookie());

            return Success(new { result.Role, result.UserId }, "Social login successful");
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
