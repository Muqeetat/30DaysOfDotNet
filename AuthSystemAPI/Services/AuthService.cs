using AuthSystemAPI.Data;
using AuthSystemAPI.DTO;
using AuthSystemAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthSystemAPI.Services
{
    public class AuthService(AppDbContext _context, IConfiguration _configuration) : IAuthService
    {
        public async Task<User> RegisterAsync(UserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            string hashed = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User {
                Email = request.Email,
                PasswordHash = hashed,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<(string Token, RefreshToken RefreshToken, User User)?> LoginAsync(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            var token = CreateToken(user);
            var refreshToken = GenerateRefreshToken();

            return (token, refreshToken, user);
        }

        public async Task<(string Token, RefreshToken RefreshToken, User User)?> RefreshTokenAsync(string oldToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == oldToken);
            if (user == null || user.TokenExpires < DateTime.Now)
                return null;

            var token = CreateToken(user);
            var refreshToken = GenerateRefreshToken();

            return (token, refreshToken, user);
        }

        public async Task UpdateUserRefreshToken(User user, RefreshToken newRefreshToken)
        {
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
            await _context.SaveChangesAsync();
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };
        }
    }
}
