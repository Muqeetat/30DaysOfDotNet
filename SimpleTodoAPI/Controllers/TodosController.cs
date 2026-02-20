using Microsoft.AspNetCore.Mvc;
using SimpleTodoAPI.DTOs;
using SimpleTodoAPI.Models;
using SimpleTodoAPI.Services;

namespace SimpleTodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This makes the URL: api/todos
    public class TodosController(ITodoService todoService) : ControllerBase
    {
        // GET: api/todos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var todos = await todoService.GetAllAsync();
            return Ok(todos); // Returns 200 OK
        }

        // GET: api/todos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var todo = await todoService.GetByIdAsync(id);
            return todo is not null ? Ok(todo) : NotFound();
        }

        // POST: api/todos
        [HttpPost]
        public async Task<IActionResult> Create(TodoCreateDto dto)
        {
            // The [ApiController] attribute handles validation automatically!
            var response = await todoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        // PUT: api/todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Todo todo)
        {
            var updated = await todoService.UpdateAsync(id, todo);
            if (!updated) return NotFound();

            return NoContent(); // 204 No Content
        }

        // DELETE: api/todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await todoService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return Ok(new { Message = "Todo deleted successfully" });
        }
    }
}