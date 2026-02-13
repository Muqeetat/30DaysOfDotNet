using SimpleTodoAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Our "In-Memory" Database
var todos = new List<Todo>();

// 1. GET all todos
app.MapGet("/todos", () => todos);

// 2. POST a new todo
app.MapPost("/todos", (Todo task) => {
    task.Id = todos.Count + 1; // Simple ID generation
    todos.Add(task);
    return Results.Created($"/todos/{task.Id}", task);
});

app.Run();