using Microsoft.EntityFrameworkCore;
using IDVerificationAPI.Models;
using IDVerificationAPI.Data;

namespace IDVerificationAPI.Services;

public class VerificationService(AppDbContext _context, ILogger<VerificationService> logger) : IVerificationService
{
    public async Task<IEnumerable<VerificationRequest>> GetAllAsync()
    {
        logger.LogInformation("Fetching all verification records from the database.");
        return await _context.VerificationRequests
        .Include(v => v.User)
         .OrderByDescending(v => v.ProcessedAt)
        .ToListAsync();
    }
    public async Task<bool> PerformExternalCheckAsync(string nationalId)
    {
        // Simulate "Network Latency" calling an external Government API
        await Task.Delay(2000);

        // Simulation Logic: 
        // Let's pretend any ID starting with "ABC" is valid, and others are fake.
        bool isValid = nationalId.StartsWith("ABC");

        if (isValid)
        {
            logger.LogInformation("Identity verification SUCCESS for ID: {NationalId}", nationalId);
        }
        else
        {
            logger.LogWarning("Identity verification FAILED for ID: {NationalId}", nationalId);
        }

        return isValid;
    }

    
}
