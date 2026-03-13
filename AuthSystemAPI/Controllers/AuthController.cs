using AuthSystemAPI.Data;
using AuthSystemAPI.DTO;
using AuthSystemAPI.Models;
using AuthSystemAPI.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthSystemAPI.Controllers
{
    [EnableRateLimiting("fixed")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto request)
        {
            await _authService.RegisterAsync(request);
            return Ok("User registered successfully!");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null) return BadRequest("Invalid email or password.");

            SetRefreshTokenCookie(result.Value.RefreshToken);
            await _authService.UpdateUserRefreshToken(result.Value.User, result.Value.RefreshToken);

            return Ok(result.Value.Token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var oldToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(oldToken)) return Unauthorized();

            var result = await _authService.RefreshTokenAsync(oldToken);
            if (result == null) return Unauthorized("Token expired or invalid.");

            SetRefreshTokenCookie(result.Value.RefreshToken);
            await _authService.UpdateUserRefreshToken(result.Value.User, result.Value.RefreshToken);

            return Ok(result.Value.Token);
        }

        private void SetRefreshTokenCookie(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
        }
    }
}