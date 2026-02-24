using Microsoft.EntityFrameworkCore;
using IDVerificationAPI.Data;
using IDVerificationAPI.Models;
using IDVerificationAPI.DTO;

namespace IDVerificationAPI.Services
{
    public class UserService(AppDbContext _context) : IUserService
    {
        public async Task<IEnumerable<User>> GetAllAsync() =>
             await _context.Users.ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            var user = new User
            {
                FullName = dto.Name,
                NationalId = dto.NationalId,
                IsVerified = false // Default value
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.FullName,
                NationalId = user.NationalId,
                IsVerified = user.IsVerified
            };
        }
        public async Task<bool> UpdateAsync(int id, User inputUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;
            user.FullName = inputUser.FullName;
            user.NationalId = inputUser.NationalId;
            user.IsVerified = inputUser.IsVerified;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}