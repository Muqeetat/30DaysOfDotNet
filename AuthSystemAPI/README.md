# AuthSystemAPI

### Step 1: Install the Security Helper

While you can write hashing logic from scratch, it is safer to use a verified library. We will use `BCrypt.Net-Next`, which is the industry standard for .NET developers.

**Run this in your Package Manager Console:**
`Install-Package BCrypt.Net-Next`

---

### Step 2: Update the User Model

Your `User` entity needs two specific fields to handle this securely.

```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    
    // This stores the hashed version, NOT the plain text
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

```

---

### Step 3: Create the Auth Controller

Create a new `AuthController.cs`. This is where the magic happens.

#### **The Register Action**

```csharp
[HttpPost("register")]
public async Task<ActionResult<User>> Register(UserDto request)
{
    // 1. Hash the password
    string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

    // 2. Create the user object
    var user = new User
    {
        Email = request.Email,
        PasswordHash = passwordHash
    };

    // 3. Save to SQL (This works locally AND in Azure now!)
    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok(user);
}

```

#### **The Login Action**

```csharp
[HttpPost("login")]
public async Task<ActionResult<string>> Login(UserDto request)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

    if (user == null) 
    {
        return BadRequest("User not found.");
    }

    // Compare the provided password with the stored hash
    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
        return BadRequest("Wrong password.");
    }

    return Ok("Success! You are logged in.");
}

```

---

### Step 4: Generate the Key

You can use a simple PowerShell command to generate a base64-encoded string that is perfect for your configuration file:

```powershell
# Generates a 32-byte (256-bit) secure random string
$bytes = New-Object Byte[] 32
(New-Object System.Security.Cryptography.RNGCryptoServiceProvider).GetBytes($bytes)
[Convert]::ToBase64String($bytes)

```

---

### Step 5: Configure the JWT Settings

Store your secret key and token details in `appsettings.json`. This keeps your credentials separate from your code logic.

```json
{
  "Jwt": {
    "Key": "YOUR_GENERATED_BASE64_KEY_HERE",
    "Issuer": "AuthSystemAPI",
    "Audience": "AuthSystemAPIUsers"
  }
}

```

---

### Step 6: Register JWT in Program.cs

To enable the "Gatekeeper," you must configure the authentication middleware to recognize and validate your tokens.

**Install the required package:**
`Install-Package Microsoft.AspNetCore.Authentication.JwtBearer`

**Add to `Program.cs` before `builder.Build()`:**

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

```

**Add to `Program.cs` after `app.UseHttpsRedirection()`:**

```csharp
app.UseAuthentication(); // Identification
app.UseAuthorization();  // Permissions

```

---

### Step 6: Implement Role-Based Access Control (RBAC)

Assign levels of authority to users by updating the `User` model and baking the role into the JWT claims.

**Update User Model:**

```csharp
public class User
{
    // ... existing properties
    public string Role { get; set; } = "User"; // Default role
}

```

**Include Role in JWT (inside AuthController):**

```csharp
private string CreateToken(User user)
{
    var claims = new List<Claim> {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.Role) // Baked-in permission
    };
    // ... generate token logic
}

```

---

### Step 7: Advanced Security with Refresh Tokens

To keep users logged in safely, we use a two-token system: a short-lived **JWT** for API calls and a long-lived **Refresh Token** stored securely in a cookie.

**1. Update the User Model again:**

```csharp
public class User
{
    // ... previous fields
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
}

```

**2. Implement Refresh Token Rotation:**
This logic (added to `AuthController`) generates a random 64-byte string and stores it as an `HttpOnly` cookie, protecting it from browser-based script attacks.

---

### Step 8: Protect Endpoints

Use the `[Authorize]` attribute to restrict access. You can lock down entire controllers or specific actions based on the roles we defined in Step 6.

```csharp
[Authorize(Roles = "Admin")]
[HttpGet("admin-dashboard")]
public IActionResult GetAdminData()
{
    return Ok("Only Admins can see this highly sensitive data.");
}

```

---

### Step 9: Implement Rate Limiting

To prevent brute-force attacks on your login or registration endpoints, we add a "Bouncer" that limits how many requests a user can make in a short window.

**Add to `Program.cs` before `builder.Build()`:**

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 10; // Max 10 requests
        options.Window = TimeSpan.FromSeconds(10); // Every 10 seconds
        options.QueueLimit = 2;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

```

**Apply to your Controller:**

```csharp
[EnableRateLimiting("fixed")]
public class AuthController : ControllerBase 
{ 
    // ...
}

```

---

### Step 10: The Middleware Pipeline

The **order** of your code in `Program.cs` is vital. It acts as a sequential filter for every request that hits your API.

```csharp
app.UseHttpsRedirection();
app.UseRouting();          // 1. Identify the route
app.UseRateLimiter();      // 2. Block spammers
app.UseAuthentication();   // 3. Verify identity (JWT)
app.UseAuthorization();    // 4. Verify permissions (Roles)
app.MapControllers();      // 5. Execute the code

```

---

### Final Testing Workflow

1. **Migration:** Run `Add-Migration SecurityUpdate` and `Update-Database`.
2. **Register:** Create a user; notice their password is encrypted in the DB.
3. **Login:** Receive your JWT and check your browser cookies for the Refresh Token.
4. **Access:** Use the Bearer token to access a protected route.
5. **Limit:** Try to spam the login 20 times to trigger the `429 Too Many Requests` response.



### Step 11: Decoupling for Clean Architecture (Day 21)

#### **1. The Interface (`IAuthService.cs`)**

This is the contract. It tells the Controller *what* can be done without showing *how*.

```csharp
public interface IAuthService
{
    Task<User> RegisterAsync(UserDto request);
    Task<(string Token, RefreshToken RefreshToken, User User)?> LoginAsync(UserDto request);
    Task<(string Token, RefreshToken RefreshToken, User User)?> RefreshTokenAsync(string oldToken);
    Task UpdateUserRefreshToken(User user, RefreshToken newRefreshToken);
}

```

#### **2. The Implementation (`AuthService.cs`)**

This is the "Chef" in the kitchen. It handles the database, BCrypt, and JWT logic.

```csharp
public class AuthService(AppDbContext _context, IConfiguration _configuration) : IAuthService
{
    public async Task<User> RegisterAsync(UserDto request)
    {
        var hashed = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User { Email = request.Email, PasswordHash = hashed };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    // ... plus Login, Refresh, and Token Generation logic ...
}

```

#### **3. The Lean Controller (`AuthController.cs`)**

The Controller now only handles HTTP concerns (status codes and cookies).

```csharp
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null) return BadRequest("Invalid credentials.");

        SetRefreshTokenCookie(result.Value.RefreshToken);
        await _authService.UpdateUserRefreshToken(result.Value.User, result.Value.RefreshToken);

        return Ok(result.Value.Token);
    }
    // ... other endpoints call _authService ...
}

```

---

### Step 12: Production-Ready Secret Management

Never hardcode your `Jwt:Key` in `appsettings.json` when pushing to GitHub. Use **Environment Variables** or **User Secrets**.

**To use User Secrets (Local Dev):**

1. Right-click your project -> **Manage User Secrets**.
2. Add your sensitive data there:

```json
{
  "Jwt": {
    "Key": "Your-Super-Secret-Unstoppable-Key-12345!"
  }
}

```

---

### Final Project Architecture Summary

By completing these steps, you have built a **Professional Grade API**:

* **Security:** Passwords are hashed, not stored.
* **Authentication:** Uses industry-standard JWT.
* **Resilience:** Rate limiting prevents brute-force attacks.
* **Maintainability:** Decoupled architecture makes the code easy to test and upgrade.

| Layer | Responsibility |
| --- | --- |
| **Controller** | Handles HTTP Requests/Responses & Cookies. |
| **Service** | Handles Business Logic, Hashing, and Tokens. |
| **Interface** | Defines the "Contract" between layers. |
| **Middleware** | Filters traffic (Auth, Rate Limiting) in a specific order. |
