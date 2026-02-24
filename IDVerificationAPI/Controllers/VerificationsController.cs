using IDVerificationAPI.Data;
using IDVerificationAPI.DTOs;
using IDVerificationAPI.Models;
using IDVerificationAPI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VerificationsController(AppDbContext db, IVerificationService _verifyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllVerified()
    {
        var todos = await _verifyService.GetAllAsync();
        return Ok(todos); 
    }

    [HttpPost]
    public async Task<IActionResult> Verify(VerifyRequestDto dto)
    {
        // 1. Find the user
        var user = await db.Users.FindAsync(dto.UserId);
        if (user == null) return NotFound("User not found.");

        // 2. Create a "Pending" record in history
        var request = new VerificationRequest 
        {
            UserId = user.Id,
            NationalIdSubmitted = dto.NationalId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        db.VerificationRequests.Add(request);
        await db.SaveChangesAsync();

        // 3. Call the "External" Mock Service
        bool isValid = await _verifyService.PerformExternalCheckAsync(dto.NationalId);

        request.ProcessedAt = DateTime.UtcNow;

        // 4. Update status based on result
        if (isValid)
        {
            user.IsVerified = true;
            request.Status = "Success";
        }
        else
        {
            request.Status = "Failed";
        }

        await db.SaveChangesAsync();

        return Ok(request);
    }
}