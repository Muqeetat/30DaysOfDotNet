using IDVerificationAPI.DTO;
using IDVerificationAPI.Models;
using IDVerificationAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace IDVerificationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This makes the URL: api/users
    public class UsersController(IUserService userService) : ControllerBase
    {
        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await userService.GetAllAsync();
            return Ok(users); // Returns 200 OK
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await userService.GetByIdAsync(id);
            return user is not null ? Ok(user) : NotFound();
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDto dto)
        {
            // The [ApiController] attribute handles validation automatically!
            var response = await userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            var updated = await userService.UpdateAsync(id, user);
            if (!updated) return NotFound();

            return NoContent(); // 204 No Content
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await userService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return Ok(new { Message = "User deleted successfully" });
        }
    }
}