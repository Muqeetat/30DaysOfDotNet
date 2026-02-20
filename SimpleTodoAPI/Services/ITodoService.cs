using SimpleTodoAPI.DTOs;
using SimpleTodoAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SimpleTodoAPI.Services
{ 
// the interface
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetAllAsync();
        Task<Todo?> GetByIdAsync(int id);
        Task<TodoResponseDto> CreateAsync(TodoCreateDto dto);
        Task<bool> UpdateAsync(int id, Todo todo);
        Task<bool> DeleteAsync(int id);
    }
}