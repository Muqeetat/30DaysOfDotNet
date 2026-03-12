using System.ComponentModel.DataAnnotations;

namespace AuthSystemAPI.DTO
{
    public class UserDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

    }
}
