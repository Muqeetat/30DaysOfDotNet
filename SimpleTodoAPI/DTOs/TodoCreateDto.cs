using System.ComponentModel.DataAnnotations;

namespace SimpleTodoAPI.DTOs;

public class TodoCreateDto
{
    [Required(ErrorMessage = "Title is mandatory")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
    public string Title { get; set; } = string.Empty;
}