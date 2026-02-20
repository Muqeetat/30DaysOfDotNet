# SimpleTodoAPI

## Day 1: Environment Setup

### Step 1: Create the GitHub Repository

1. Log into **GitHub**.
2. Click the **+** icon in the top right and select **New repository**.
3. **Repository name:** `30DaysOfDotNet`
4. **Public/Private:** Your choice (Public is great for accountability!).
5. **Initialize:** Check "Add a README file" and "Add .gitignore" (choose the **VisualStudio** template).
6. Click **Create repository**, then click the green **Code** button to copy the HTTPS/SSH URL.

---

### Step 2: Create the Web API Project

Open your terminal (PowerShell, CMD, or Bash) and run these commands.


```bash
# 1. Clone your new repo (replace with your URL)
git clone https://github.com/YOUR_USERNAME/30DaysOfDotNet.git

# 2. Move into the folder
cd 30DaysOfDotNet

# 3. Create a new Web API project
# We use the '-o' flag to put it in a folder named 'src' or 'api' to keep things tidy
dotnet new webapi -o SimpleTodoAPI

# 4. Move into the project folder to test it
cd SimpleTodoAPI
dotnet run

```

---

### Step 3: Push to GitHub

Now, let's send that code up to the cloud. From your main `30DaysOfDotNet` directory:

```bash
# Stage all changes
git add .

# Commit with a Day 1 message
git commit -m "Day 1: Initial Web API setup"

# Push to the main branch
git push origin main

```

## Day 2 : Build Your API

### Step 1: Define the Model

Open your project in VS Code or Visual Studio. Create a new file called `Todo.cs`:

```csharp
public class Todo
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsCompleted { get; set; }
}

```

---

### Step 2: Set up the Logic in `Program.cs`

Replace the contents of your `Program.cs` with the following code. We’ll use an in-memory `List` to act as our "database" for today.

```csharp
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

```

---

### Step 3: Test with Postman

1. **Run the App:** In your terminal, type `dotnet run`. .
2. **The GET Request:**
* Open Postman.
* Set method to **GET**.
* URL: `https://localhost:xxxx/todos`
* Hit **Send**. You should see an empty array `[]`.


3. **The POST Request:**
* Set method to **POST**.
* URL: `https://localhost:xxxx/todos`
* Go to the **Body** tab -> Select **raw** -> Select **JSON**.
* Paste this:
```json
{
  "title": "Finish Day 2 of .NET challenge",
  "isCompleted": false
}

```

* Hit **Send**. You should see the object returned with an `id: 1`.

---

### Git Check-in

```bash
git add .
git commit -m "Day 2: Implemented GET and POST for Todo API"
git push origin main

```
---
## Day 3: Connect to Database

### Step 1: Install the Packages

You need three specific tools to help .NET talk to SQL Server and manage the database. Run these in your terminal:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef

```

---

### Step 2: Create the Database Context

The **DbContext** is the bridge between your C# code and the SQL Database. Create a file named `AppDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // This represents your table
    public DbSet<Todo> Todos => Set<Todo>();
}

```

---

### Step 3: Register and Connect (Program.cs)

Now, tell the app to use SQL Server. Replace your `builder.Build()` area with this:

```csharp
// Add this at the top
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connect to SQL Server (LocalDB is usually installed with Visual Studio)
var connectionString = "Server=(localdb)\\mssqllocaldb;Database=TodoDb;Trusted_Connection=True;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

```

---

### Step 4: Migrations (The Database Build)

A **Migration** is a script that tells SQL Server how to create the tables to match your C# `Todo` model.

Run these two commands:

1. **Create the script:** `dotnet ef migrations add InitialCreate`
2. **Apply to Database:** `dotnet ef database update`

---

### Step 5: Update Endpoints to use the DB

Now, inject the `AppDbContext` into your routes. .NET will handle the database connection for you.

```csharp
// GET: From the database
app.MapGet("/todos", async (AppDbContext db) => 
    await db.Todos.ToListAsync());

// POST: Save to the database
app.MapPost("/todos", async (Todo todo, AppDbContext db) => 
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

```

---
## Day 4: CRUD Complete

### Step 1: Add the PUT (Update) Endpoint

The `PUT` verb is used to replace an existing resource. In `Program.cs`, add this below your Day 3 routes:

```csharp
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

```

---

### Step 2: Add the DELETE Endpoint

This is the simplest one, but the most "dangerous." Always verify the ID exists first!

```csharp
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

```

---

### Step 3: Global Error Handling (The "Pro" Way)

Right now, if your database goes down, your API returns a messy, scary error page to the user. Let’s add a safety net.

In `Program.cs`, right after `var app = builder.Build();`, add this:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Shows detailed errors only during development
}
else
{
    app.UseExceptionHandler("/error"); // Generic error page for production
}

```

---

### Step 4: Testing in Postman

1. **PUT:** Change the URL to `https://localhost:xxxx/todos/1`. In the **Body**, change the `isCompleted` to `true`. Hit Send. You should get a **204 No Content**.
2. **DELETE:** Change the method to **DELETE** and use the URL `https://localhost:xxxx/todos/1`. Hit Send. You should get a **200 OK**.
3. **GET:** Run your "Get All" again to verify the list is empty.

---

### Git Check-in

Time to save the full set!

```bash
git add .
git commit -m "Day 4: Completed full CRUD with Error Handling"
git push origin main

```

## Day 5: Clean Architecture Basics Refactor

### The "Clean" Folder Structure

Before writing code, create these folders in your project:

* `Models/` (Your data blueprints)
* `Data/` (Your database context)
* `Services/` (Your business logic/brains)
* `Controllers/` (Your traffic cops)

---

### 1. The Data Layer (`Data/` & `Models/`)

Move your `Todo` class to `Models/Todo.cs` and your `AppDbContext` to `Data/AppDbContext.cs`. Ensure you update the **namespaces** at the top of these files!

---

### 2. The Service Layer (`Services/`)

This is a Best Practice. Your controller shouldn't talk directly to the database; it should ask a **Service**.

**Define the Interface (`Services/ITodoService.cs`):**

```csharp
public interface ITodoService {
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task CreateAsync(Todo todo);
    Task<bool> UpdateAsync(int id, Todo todo);
    Task<bool> DeleteAsync(int id);
}

```

**Implement the Logic (`Services/TodoService.cs`):**

```csharp
using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Data;
using SimpleTodoAPI.Models;


namespace SimpleTodoAPI.Services
{
    public class TodoService(AppDbContext db) : ITodoService
    {
        // GET ALL
        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            return await db.Todos.ToListAsync();
        }

        // GET BY ID
        public async Task<Todo?> GetByIdAsync(int id)
        {
            return await db.Todos.FindAsync(id);
        }

        // CREATE
        public async Task CreateAsync(Todo todo)
        {
            db.Todos.Add(todo);
            await db.SaveChangesAsync();
        }

        // UPDATE
        public async Task<bool> UpdateAsync(int id, Todo inputTodo)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return false;

            todo.Title = inputTodo.Title;
            todo.IsCompleted = inputTodo.IsCompleted;

            await db.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return false;

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return true;
        }
    }
}

```

---

### 3. The Controller Layer (`Controllers/`)

We are moving away from `app.MapGet` and toward **Controllers**. This keeps your routing organized.

**`Controllers/TodosController.cs`**:

```csharp
namespace SimpleTodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This makes the URL: api/todos
    public class TodosController(ITodoService todoService) : ControllerBase
    {
        // GET: api/todos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var todos = await todoService.GetAllAsync();
            return Ok(todos); // Returns 200 OK
        }

        // GET: api/todos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var todo = await todoService.GetByIdAsync(id);
            return todo is not null ? Ok(todo) : NotFound();
        }

        // POST: api/todos
        [HttpPost]
        public async Task<IActionResult> Create(Todo todo)
        {
            await todoService.CreateAsync(todo);
            // Best practice: returns 201 Created and the URL to find the new item
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
        }

        // PUT: api/todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Todo todo)
        {
            var updated = await todoService.UpdateAsync(id, todo);
            if (!updated) return NotFound();

            return NoContent(); // 204 No Content
        }

        // DELETE: api/todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await todoService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return Ok(new { Message = "Todo deleted successfully" });
        }
    }
}

```

---

### 4. Wiring it up (Dependency Injection in `Program.cs`)

Now your `Program.cs` becomes very short. Its only job is "Registration."

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register the Database
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our Service (The "Magic" of DI)
builder.Services.AddScoped<ITodoService, TodoService>();

// Add support for Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers(); // This replaces all your MapGet/MapPost lines!

app.Run();

```

---

### 5: Connection Strings

Instead of hardcoding the server name in `Program.cs`, move it to `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TodoDb;Trusted_Connection=True;"
}

```

---
### 6: The "Clean Slate" Fix (Recommended)

Since this is an "InitialCreate" migration (your very first one) and you are likely still in development, the easiest way to resolve namespace mismatches is to simply start fresh.

* **Delete the Migrations Folder:** Manually delete the entire `Migrations` folder in your solution explorer.
* **Fix your Code:** Ensure your `AppDbContext` is exactly where you want it and has the correct namespace.
* **Re-run the Migration:**
* **CLI:** `dotnet ef migrations add InitialCreate`
* **PMC:** `Add-Migration InitialCreate`


---
### 7: The "Base URL" Check

By default, the `[Route("api/[controller]")]` attribute in your `TodosController` adds an **`api/`** prefix.

* **Old URL:** `http://localhost:5xxx/todos`
* **New URL:** `http://localhost:5xxx/api/todos`

---

## Day 6: Refactor code and Review Naming

---

### 1. Creating the DTOs

Create a new folder `DTOs/` and add these files:

**`DTOs/TodoCreateDto.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace SimpleTodoAPI.DTOs;

public class TodoCreateDto
{
    [Required(ErrorMessage = "Title is mandatory")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
    public string Title { get; set; } = string.Empty;
}

```

**`DTOs/TodoResponseDto.cs`**

```csharp
namespace SimpleTodoAPI.DTOs;

public class TodoResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

```

---

### 2. Update the Service Logic

Your Service now needs to handle the conversion. In a real-world app, we use a tool called **AutoMapper**, but for Day 6, we will do it manually to understand the logic.

**In `ITodoService.cs`:**

```csharp
Task<TodoResponseDto> CreateAsync(TodoCreateDto dto);

```

**In `TodoService.cs`:**

```csharp
public async Task<TodoResponseDto> CreateAsync(TodoCreateDto dto)
{
    var todo = new Todo 
    { 
        Title = dto.Title, 
        IsCompleted = false // Default value
    };

    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return new TodoResponseDto 
    { 
        Id = todo.Id, 
        Title = todo.Title, 
        IsCompleted = todo.IsCompleted 
    };
}

```

---

### 3. The Controller (Validation)

Because you added `[ApiController]` yesterday, .NET will **automatically** check the `[Required]` and `[StringLength]` tags. If the user sends an empty title, the API will return a **400 Bad Request** without you writing a single `if` statement!

**`Controllers/TodosController.cs`**

```csharp
[HttpPost]
public async Task<IActionResult> Create(TodoCreateDto dto)
{
    // The [ApiController] attribute handles validation automatically!
    var response = await todoService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
}

```

---

### Summary of Day 6

1. **Data Annotations:** We used `[Required]` and `[StringLength]` to protect the database from "junk" data.
2. **DTOs:** We created a specific "Input" object (`TodoCreateDto`) and "Output" object (`TodoResponseDto`).
3. **Security:** We prevented "Overposting" because the user no longer has direct access to the `Todo` model properties via the API.

---

### Test it in Postman!

1. Try to send a `POST` request with an empty title `{ "title": "" }`. You should get a **400 Bad Request** with a helpful error message.
2. Try to send an extra field like `{ "title": "Test", "id": 999 }`. Notice that the API ignores the `id` because our DTO doesn't have that property.

---

## Day 7 

### 1. The Refactor Checklist

Go through your files and check for these three common "smells":

* **Namespace Consistency:** Since you moved files into folders on Day 5, ensure every file has a consistent namespace (e.g., `SimpleTodoAPI.Services`, `SimpleTodoAPI.Controllers`).
* **Expression-Bodied Members:** For simple one-line methods in your Service, use the `=>` syntax to make it sleeker.
* *Before:* `public async Task<IEnumerable<Todo>> GetAll() { return await _db.Todos.ToListAsync(); }`
* *After:* `public async Task<IEnumerable<Todo>> GetAll() => await _db.Todos.ToListAsync();`


* **Primary Constructors:** Ensure you are using the modern C# 12+ syntax for your Service and Controller to reduce boilerplate code.

---

### 2. Improve Naming

Naming is one of the hardest things in programming. Let's make yours "Industry Standard":

| Old Name | Better Name | Why? |
| --- | --- | --- |
| `inputTodo` | `todoDto` | Clearly identifies it as a Data Transfer Object. |
| `GetAll()` | `GetTodosAsync()` | Clearly indicates it returns multiple and is asynchronous. |
| `db` | `_context` | A standard naming convention for the Database Context. |
| `todo` (in loop) | `existingTodo` | Distinguishes between what's in the DB vs. what the user sent. |

---

### 3. Write a Killer README

Your GitHub repository is your resume. A good `README.md` makes a huge difference. Use this template:

---

### 4. Push the Clean Version

Time to finalize your work. Run these commands to ensure your Git history is clean:

```bash
# Verify everything is saved
git status

# Add and commit with a meaningful message
git add .
git commit -m "Day 7: Refactored architecture, added DTOs, and updated documentation"

# Push to your repository
git push origin main

```

---