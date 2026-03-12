using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        // Anyone with a valid token (User or Admin)
        [HttpGet("user-data"), Authorize(Roles = "User,Admin")]
        public IActionResult GetUserData()
        {
            return Ok("This is data for all registered users.");
        }

        // ONLY Admins
        [HttpGet("admin-only"), Authorize(Roles = "Admin")]
        public IActionResult GetAdminSettings()
        {
            return Ok("Welcome, Boss. Here are the admin settings.");
        }
    }
}
