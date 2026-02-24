namespace IDVerificationAPI.DTO
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }
}
