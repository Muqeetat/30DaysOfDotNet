using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Data;
using SimpleTodoAPI.Services;
using Scalar.AspNetCore; 

var builder = WebApplication.CreateBuilder(args);

// Register the Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// This generates the OpenAPI document (the JSON)
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // This exposes the JSON file at /openapi/v1.json
    app.MapOpenApi();

    // 2. This creates the visual UI at /scalar/v1
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => "SimpleTodoAPI is running");

app.Run();