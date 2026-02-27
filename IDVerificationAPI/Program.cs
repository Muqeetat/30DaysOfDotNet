using IDVerificationAPI.Data;
using IDVerificationAPI.Middleware;
using IDVerificationAPI.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Register the Database
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var connectionString = builder.Configuration.GetConnectionString("AzureDbConnection")
                    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// This generates the OpenAPI document (the JSON)
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVerificationService, VerificationService>(); ;

var app = builder.Build();

// 1. Add this first so it catches errors from everything below it
app.UseMiddleware<ExceptionMiddleware>();

app.MapOpenApi();
app.MapScalarApiReference();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();

    //// 2. This creates the visual UI at /scalar/v1
    //app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();

// Automatic Migration on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>(); // Use your DbContext name
        context.Database.Migrate();
        Console.WriteLine("Database migration applied successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred migrating the DB: {ex.Message}");
    }
}

app.Run();