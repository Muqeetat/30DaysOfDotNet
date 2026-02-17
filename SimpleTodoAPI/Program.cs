using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES (Registration)
// Connect to SQL Server (LocalDB is usually installed with Visual Studio)
var connectionString = "Server=(localdb)\\mssqllocaldb;Database=TodoDb;Trusted_Connection=True;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Shows detailed errors only during development
}
else
{
    app.UseExceptionHandler("/error"); // Generic error page for production
}

// 2. ENDPOINTS (Place your code here!)

// GET: From the database
app.MapGet("/todos", async (AppDbContext db) =>
    await db.Todos.ToListAsync());  //Successful :

// POST: Save to the database
app.MapPost("/todos", async (Todo todo, AppDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo); //Successful: 201 Created
});

// UPDATE a todo
app.MapPut("/todos/{id}", async (int id, Todo inputTodo, AppDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound($"Todo with ID {id} not found.");

    // Update the properties
    todo.Title = inputTodo.Title;
    todo.IsCompleted = inputTodo.IsCompleted;

    await db.SaveChangesAsync();

    return Results.NoContent(); // 204 No Content is the standard for successful updates
});

// DELETE a todo
app.MapDelete("/todos/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
});

// 3. RUN
app.Run();