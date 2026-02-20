namespace IDVerificationAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty; // The ID number we will verify
        public bool IsVerified { get; set; } = false;
    }
}
