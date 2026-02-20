using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Data;
using SimpleTodoAPI.Services;


var builder = WebApplication.CreateBuilder(args);

// Register the Database
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add support for Controllers
builder.Services.AddControllers();

// Registering the service:
// "Scoped" means a new instance is created for every single HTTP request.
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers(); // This replaces all your MapGet/MapPost lines!

app.Run();