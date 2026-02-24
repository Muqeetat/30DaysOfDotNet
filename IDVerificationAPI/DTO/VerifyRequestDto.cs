namespace IDVerificationAPI.DTOs;

public class VerifyRequestDto
{
    public int UserId { get; set; }
    public string NationalId { get; set; } = string.Empty;
}
