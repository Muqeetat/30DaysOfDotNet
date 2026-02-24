using IDVerificationAPI.Models;

namespace IDVerificationAPI.Services;

public interface IVerificationService
{
    Task<IEnumerable<VerificationRequest>> GetAllAsync();
    Task<bool> PerformExternalCheckAsync(string nationalId);
}
