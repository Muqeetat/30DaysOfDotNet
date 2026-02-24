using IDVerificationAPI.Data;
using IDVerificationAPI.Middleware;
using IDVerificationAPI.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Register the Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// This generates the OpenAPI document (the JSON)
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVerificationService, VerificationService>(); ;

var app = builder.Build();

// 1. Add this first so it catches errors from everything below it
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // 2. This creates the visual UI at /scalar/v1
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();