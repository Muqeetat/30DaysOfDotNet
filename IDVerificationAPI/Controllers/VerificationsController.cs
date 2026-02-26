using Microsoft.EntityFrameworkCore;
using IDVerificationAPI.Data;
using IDVerificationAPI.DTOs;
using IDVerificationAPI.Models;
using IDVerificationAPI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VerificationsController(AppDbContext _context, IVerificationService _verifyService) : ControllerBase
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
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null) return NotFound("User not found.");

        if (user.IsVerified)
        {
            return BadRequest(new
            {
                message = "This user is already verified. No further action needed."
            });
        }

        // Stop them from clicking the button twice while the first one is still processing
        var pendingRequest = await _context.VerificationRequests
            .AnyAsync(v => v.UserId == dto.UserId && v.Status == "Pending");

        if (pendingRequest)
        {
            return Conflict("A verification is already in progress. Please wait.");
        }

        // 2. Create a "Pending" record in history
        var request = new VerificationRequest 
        {
            UserId = user.Id,
            NationalIdSubmitted = dto.NationalId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.VerificationRequests.Add(request);
        await _context.SaveChangesAsync();

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

        await _context.SaveChangesAsync();

        return Ok(request);
    }
}