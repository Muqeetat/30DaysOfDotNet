namespace SimpleTodoAPI.DTOs;

public class TodoResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}