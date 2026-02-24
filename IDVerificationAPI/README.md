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