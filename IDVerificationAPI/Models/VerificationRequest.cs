namespace IDVerificationAPI.Models
{
    public class VerificationRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = "Pending"; // Simple string for now: Pending, Success, Failed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
