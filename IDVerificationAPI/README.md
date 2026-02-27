# IDVerificationAPI

A secure backend system built with **.NET 10** designed to simulate identity verification processes (like VerifyMe or KYC providers). This project tracks user identities and maintains a strict audit trail of verification attempts.

---

## Project Roadmap & Evolution

### Day 8: Initial Project Setup & Modeling

The goal was to establish the core data structures and link the API to a SQL Server database.

#### **Step 1: Project Initialization**

```bash
# Create the Web API project
dotnet new webapi -o IDVerificationAPI
cd IDVerificationAPI

```

#### **Step 2: Defining Simple Models**

I focused on a decoupled data model where a **User** is the identity, and a **VerificationRequest** is the historical event.

* **User.cs**: Stores identity basics (`FullName`, `NationalId`) and the current verification state.
* **VerificationRequest.cs**: Links to the user to track specific attempts, statuses (`Pending`, `Success`, `Failed`), and timestamps.

#### **Step 3: Database Integration**

I utilized **Entity Framework Core** with the following steps:

1. **Installed Packages**: `Microsoft.EntityFrameworkCore.SqlServer` and `Design`.
2. **Configured DbContext**: Created `AppDbContext` to manage the `Users` and `VerificationRequests` tables.
3. **Connection String**: Set up a local SQL connection in `appsettings.json`.
4. **Migrations**:
```bash
dotnet ef migrations add InitialIdentitySetup
dotnet ef database update

```

---

### Day 9: Mocking External Services

I implemented the "VerifyMe" logic by simulating an external API call.

* **Decoupling**: Created `IVerificationService` so the Controller doesn't handle the "how" of verification.
* **Simulation**: Added `Task.Delay(2000)` to replicate the real-world latency of checking government databases.

---

### Day 10: State Management & Timestamps

Improved the system's "memory" by adding detailed logging.

* **Persistence**: Every attempt is now saved to the database with a `Status` and `ProcessedAt` timestamp.
* **Audit Trail**: This ensures that even if an external check fails, the record of *when* and *why* remains in the system for admin review.

---

## Key Technical Skills Demonstrated

* **Separation of Concerns**: Moving logic from Controllers to Services.
* **Async Programming**: Using `async/await` for non-blocking I/O operations.
* **DTOs (Data Transfer Objects)**: Preventing overposting attacks by using specific input models.
* **Audit Logging**: Implementing timestamps for tracking system performance.

---

## How to Run & Test

1. Ensure **SQL Server (LocalDB)** is running.
2. Run `dotnet ef database update`.
3. Run `dotnet run`.
4. **The Logic**:
* IDs starting with **"ABC"** will return **Success**.
* All other IDs will return **Failed**.

---

## Testing with Postman

To verify the **IDVerificationAPI** logic, follow these steps in Postman.

### 1. Create a User

Before verifying, you need a user in the database.

* **Method:** `POST`
* **URL:** `https://localhost:xxxx/api/users`
* **Body (JSON):**

```json
{
  "fullName": "John Doe",
  "nationalId": "ABC12345"
}

```

### 2. Run Verification (Success Case)

Use a National ID starting with **"ABC"** to trigger a successful mock verification.

* **Method:** `POST`
* **URL:** `https://localhost:xxxx/api/verify`
* **Body (JSON):**

```json
{
  "userId": 1,
  "nationalId": "ABC12345"
}

```

* **Expected Result:** `200 OK` with `status: "Success"` after a 2-second delay.

### 3. Run Verification (Failure Case)

Use any other ID format to test the "Failed" logic and database logging.

* **Method:** `POST`
* **URL:** `https://localhost:xxxx/api/verify`
* **Body (JSON):**

```json
{
  "userId": 1,
  "nationalId": "XYZ98765"
}

```

* **Expected Result:** `200 OK` with `status: "Failed"`.

---

### Day 11: GET Verification Endpoint

* **Feature:** Implemented a system-wide "All Verifications" endpoint.
* **Logic:** Integrated **Eager Loading** (`.Include`) to ensure that verification logs are paired with the corresponding User identity data.
* **Sorting:** Applied descending chronological sorting so that the most recent ID checks appear at the top of the dashboard.

---

### Day 12: Add built-in logging

* **Feature:** Integrated `ILogger<T>` into the service layer.
* **Implementation:** Added strategic logging for the verification lifecycle (Start, Success, Failure).

---

### Day 13: Global Exception Middleware

Feature: Implemented a centralized ExceptionMiddleware to catch all unhandled errors.

Implementation: Developed a custom ErrorResponse model to standardize API error outputs.

Security: Configured environment-based error reporting to hide sensitive stack traces in production while allowing full debugging in development.

Congratulations! You’ve reached the end of Week 2. You have a functional, logged, and error-protected **IDVerificationAPI**. Day 14 is about moving your code from "It works on my machine" to "It works on the internet."

Deploying to **Azure App Service** (Free Tier) is the industry standard for .NET apps.

---
##  Day 14: Deployment to Azure App Service (Free Tier)

### 1. Step-by-Step Deployment (Visual Studio)

#### **Step A: The Publish Wizard**

1. **Right-click** your Project (the API project) in Solution Explorer and select **Publish**.
2. **Target:** Select **Azure**, then click Next.
3. **Specific Target:** Select **Azure App Service (Windows)**. (Windows is generally easier for initial .NET deployments).
4. **App Service:** * Click the **+ (Create New)** button.
* **Name:** Give it a unique name (e.g., `id-verify-api-yourname`).
* **Hosting Plan:** Click **New**. Change the **Size** to **Free (F1)**. *This is the most important step to avoid charges.*
* Click **Create**. Wait for Azure to provision the "house" for your code.



#### **Step B: Setting up the Database**

In the same Publish screen, you’ll see a section for **Service Dependencies**.

1. Click the **+** or **Connect** next to **SQL Server Database**.
2. Select **Azure SQL Database** -> Next.
3. Click **Create New**:
* **Database Name:** `IDVerificationDb`.
* **Database Server:** Create a new server. *Keep your admin username and password safe!*
* **Note:** If prompted for a firewall rule, select **"Add my client IP"** so your local Visual Studio can talk to it.


4. Once created, Visual Studio will ask for the **Connection String name**. Ensure it matches the one in your `appsettings.json` (usually `DefaultConnection`).

#### **Step C: Migrations in the Cloud**

Visual Studio can run your migrations during the push:

1. On the Publish Summary page, click **More Actions** -> **Edit**.
2. Go to the **Settings** tab.
3. Expand **Entity Framework Migrations**.
4. Check the box: **Apply this migration on publish**.
5. Click **Save**.

---

### 3. The Big Moment: "Publish"

Click the big **Publish** button at the top of the summary screen.

* Visual Studio will build your project in **Release** mode.
* It will zip up your files and send them to Azure.
* It will run `dotnet ef database update` on the Azure SQL instance.
* Your browser will open automatically to your new URL: `https://your-app-name.azurewebsites.net/swagger`.

---

### 4. Verification Checklist

Once live, test these three things to ensure the "Day 14" mission is complete:

1. **Swagger UI:** Does it load at the `/swagger` URL?
2. **Database Connection:** Try the `POST /api/users` endpoint. Does it successfully save a user to the cloud database?
3. **Middleware:** Trigger an error (e.g., fetch a User ID that doesn't exist). Does your custom `ExceptionMiddleware` still return a clean JSON error?

---

### 🛡️ Deployment Pro-Tip

**Hidden Secrets:** Do **not** hardcode your Azure SQL password in `appsettings.json`. Instead, go to the **Azure Portal** -> **Your App Service** -> **Configuration**. Add your connection string there. Azure will automatically inject it into your app, keeping your password out of your source code!

**Would you like me to show you how to set up those environment variables in the Azure Portal to keep your connection string secret?**

