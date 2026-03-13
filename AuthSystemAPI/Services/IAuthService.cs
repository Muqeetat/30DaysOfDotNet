using AuthSystemAPI.DTO;
using AuthSystemAPI.Models;

namespace AuthSystemAPI.Services
{
    public interface IAuthService
    {
        // Task to handle user registration
        Task<User> RegisterAsync(UserDto request);

        // Task to handle login, returning the JWT, RefreshToken object, and the User entity
        Task<(string Token, RefreshToken RefreshToken, User User)?> LoginAsync(UserDto request);

        // Task to handle token renewal using the existing refresh token string
        Task<(string Token, RefreshToken RefreshToken, User User)?> RefreshTokenAsync(string oldToken);

        // Task to persist the new refresh token details to the database
        Task UpdateUserRefreshToken(User user, RefreshToken newRefreshToken);
    }
}