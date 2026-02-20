# IDVertificationAPI

## Day 8: Project Setup

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
public class Todo
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

```