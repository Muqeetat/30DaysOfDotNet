# 30DaysOfDotNet


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

### What you learned today:

* **Route Parameters:** You learned how to use `{id}` in a URL so the API knows *which* specific item you are talking about.
* **State Management:** You learned how to find a record, modify its properties in memory, and then tell EF Core to "Sync" those changes back to SQL using `SaveChangesAsync`.
* **Standard Status Codes:** * `404 Not Found`: For when an ID doesn't exist.
* `204 No Content`: For successful updates where you don't need to send the whole object back.



---

### Git Check-in

Time to save the full set!

```bash
git add .
git commit -m "Day 4: Completed full CRUD with Error Handling"
git push origin main

```