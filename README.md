 # SimpleTodoAPI (.NET 10)

A clean, production-ready Todo API built over 30 days to master .NET fundamentals.

## Features
- **Clean Architecture:** Separation of Concerns with Controllers, Services, and DTOs.
- **Persistence:** SQL Server integration via Entity Framework Core.
- **Validation:** Data Annotations and DTOs to prevent overposting.
- **Async/Await:** Fully non-blocking database operations.

## Tech Stack
- .NET 10 (C# 14)
- EF Core (SQL Server)
- Postman

## How to Run
1. Clone the repo.
2. Update `appsettings.json` with your Connection String.
3. Run `dotnet ef database update`.
4. Press `F5` or `dotnet run`.