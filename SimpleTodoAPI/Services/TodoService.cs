using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Data;
using SimpleTodoAPI.DTOs;
using SimpleTodoAPI.Models;


namespace SimpleTodoAPI.Services
{
    public class TodoService(AppDbContext db) : ITodoService
    {
        // GET ALL
        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            return await db.Todos.ToListAsync();
        }

        // GET BY ID
        public async Task<Todo?> GetByIdAsync(int id)
        {
            return await db.Todos.FindAsync(id);
        }

        // CREATE
        public async Task<TodoResponseDto> CreateAsync(TodoCreateDto dto)
        {
            var todo = new Todo
            {
                Title = dto.Title,
                IsCompleted = false // Default value
            };

            db.Todos.Add(todo);
            await db.SaveChangesAsync();

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
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return false;

            todo.Title = inputTodo.Title;
            todo.IsCompleted = inputTodo.IsCompleted;

            await db.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return false;

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return true;
        }
    }
}