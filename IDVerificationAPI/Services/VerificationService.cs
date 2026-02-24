using Microsoft.EntityFrameworkCore;
using IDVerificationAPI.Models;
using IDVerificationAPI.Data;

namespace IDVerificationAPI.Services;

public class VerificationService(AppDbContext _context) : IVerificationService
{
    public async Task<IEnumerable<VerificationRequest>> GetAllAsync()=>
        
        await _context.VerificationRequests
        .Include(v => v.User) // 👈 Important: Joins the User table so you see who was verified
         .OrderByDescending(v => v.ProcessedAt)
        .ToListAsync();
 
    public async Task<bool> PerformExternalCheckAsync(string nationalId)
    {
        // Simulate "Network Latency" calling an external Government API
        await Task.Delay(2000);

        // Simulation Logic: 
        // Let's pretend any ID starting with "ABC" is valid, and others are fake.
        if (nationalId.StartsWith("ABC"))
        {
            return true;
        }
        return false;
    }

    
}
