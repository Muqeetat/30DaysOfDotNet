using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Data;
using SimpleTodoAPI.DTOs;
using SimpleTodoAPI.Models;


namespace SimpleTodoAPI.Services
{
    public class TodoService(AppDbContext _context) : ITodoService
    {
        // GET ALL
        public async Task<IEnumerable<Todo>> GetAllAsync() =>
             await _context.Todos.ToListAsync();
        

        // GET BY ID
        public async Task<Todo?> GetByIdAsync(int id) =>
            await _context.Todos.FindAsync(id);
        

        // CREATE
        public async Task<TodoResponseDto> CreateAsync(TodoCreateDto dto)
        {
            var todo = new Todo
            {
                Title = dto.Title,
                IsCompleted = false // Default value
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted
            };
        }

        // UPDATE
        public async Task<bool> UpdateAsync(int id, Todo inputTodo)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo is null) return false;

            todo.Title = inputTodo.Title;
            todo.IsCompleted = inputTodo.IsCompleted;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo is null) return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}