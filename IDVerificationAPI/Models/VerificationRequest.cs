namespace IDVerificationAPI.Models
{
    public class VerificationRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = "Pending"; // Simple string for now: Pending, Success, Failed

        public string NationalIdSubmitted { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; } // When the "External API" finished

        public User? User { get; set; }

    }
}
