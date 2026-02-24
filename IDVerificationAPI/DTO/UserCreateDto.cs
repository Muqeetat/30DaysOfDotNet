using System.ComponentModel.DataAnnotations;

namespace IDVerificationAPI.DTO
{
    public class UserCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string NationalId { get; set; } = string.Empty;
    }
}
