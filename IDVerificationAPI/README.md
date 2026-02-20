# IDVertificationAPI

## Day 8: Project Setup

In this system, a **User** is the person, and a **Verification** is an "event" or a "request" to prove who they are.


### Step 1: Create the Web API Project

Open your terminal (PowerShell, CMD, or Bash) and run these commands.

```bash

# 1. Move into the folder
cd 30DaysOfDotNet

# 2. Create a new Web API project
dotnet new webapi -o IDVerificationAPI

# 3. Move into the project folder to test it
cd IDVerificationAPI
dotnet run

```

---

### Step 2: Push to GitHub

From your main `30DaysOfDotNet` directory:

```bash

git add .
git commit -m "Day 8: Initial IDVerfication API setup"
git push origin main

```

### Step 3: Define the Models

1. Create a new file called `User.cs`:

```csharp

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty; // The ID number we will verify
    public bool IsVerified { get; set; } = false;
}

```

---

2. Create a new file caled `VerificationRequest.cs`:

This tracks the attempt to verify a user. It links back to the User.

```csharp

namespace IDVerificationAPI.Models;

public class VerificationRequest
{
    public int Id { get; set; }
    public int UserId { get; set; } 
    public string Status { get; set; } = "Pending"; // Simple string for now: Pending, Success, Failed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

```

---

### Step 4: Install the Packages

Run these in your terminal:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef

```

---

### Step 5: Create the Database Context

Create a file named `AppDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();
}

```

---

### Step 6: Connection Strings

Add this to `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IDVerificationDb;Trusted_Connection=True;"
}

```

---

### Step 7: Run your migration commands:
```bash
dotnet ef migrations add AddUserAndVerificationModels
dotnet ef database update

```