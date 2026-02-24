namespace IDVerificationAPI.Models;
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; } // Only show this in Development mode
}
