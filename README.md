# Projects Overview & Testing Guide

This repository contains a series of projects tracking my journey from building basic REST APIs to implementing enterprise-grade security and high-availability systems.

## 🛠 Tech Stack

* **Framework:** .NET 10 (C# 14)
* **Database:** EF Core (SQL Server)
* **Testing:** Postman / Scalar API Reference
* **Security:** JWT, BCrypt, Rate Limiting

## ⚙️ How to Run

1. **Clone the repo.**
2. **Update `appsettings.json**` with your local SQL Connection String.
3. **Run Migrations:** Open your terminal in the project folder and run:
```bash
dotnet ef database update

```
4. **Launch:** Press `F5` in Visual Studio or run `dotnet run`.

---

## Project 1: SimpleTodoAPI

**Goal:** Master the fundamentals of RESTful architecture and Entity Framework Core.
This project focuses on basic data persistence and clean endpoint routing.

### **How to Test:**

* **GET all tasks:** `GET /api/todo`
* **Create a task:** `POST /api/todo`
* Body: `{ "title": "Buy milk", "isCompleted": false }`


* **Update task:** `PUT /api/todo/{id}`
* **Delete task:** `DELETE /api/todo/{id}`

---

## Project 2: ID Verification System

### **Testing with Postman**

#### **1. Create a User**

Before verifying, you need a user in the database.

* **Method:** `POST`
* **URL:** `https://localhost:xxxx/api/users`
* **Body:**
```json
{ "fullName": "John Doe", "nationalId": "ABC12345" }

```

#### **2. Run Verification (Success Case)**

Use a National ID starting with **"ABC"** to trigger a successful mock verification.

* **Method:** `POST`
* **URL:** `https://localhost:xxxx/api/verify`
* **Body:**
```json
{ "userId": 1, "nationalId": "ABC12345" }

```
* **Expected Result:** `200 OK` with `status: "Success"` after a 2-second simulated processing delay.

#### **3. Run Verification (Failure Case)**

Use any other ID format to test the "Failed" logic and database logging.

* **Expected Result:** `200 OK` with `status: "Failed"`.

---

### Project 3: AuthSystemAPI (Advanced Security)

**Goal:** Implementation of multi-layer defense, including JWT, Refresh Token Rotation, and Rate Limiting.

#### **How to Test the Security Flow**

**1. User Registration**

* **Endpoint:** `POST /api/auth/register`
* **Action:** Send a JSON body with an email and password.
* **Verification:** Check your SQL database. You will see that the password is **hashed via BCrypt**, making it unreadable to anyone with database access.

**2. Login (The Dual-Token Handshake)**

* **Endpoint:** `POST /api/auth/login`
* **The Result:** * **Body:** You receive a long string (the **JWT Access Token**).
* **Hidden Action:** Your browser/Postman receives a `Set-Cookie` header containing the **Refresh Token**.
* *Tip:* In Postman, look at the "Cookies" tab; in a browser, check DevTools (F12) -> Application -> Cookies.


**3. Test Authorization & RBAC**

* **Accessing Data:** Copy the JWT and add it to the header: `Authorization: Bearer <token>`.
* **Verification:** * Access a `[Authorize]` route as a standard User.
* Access an **Admin** route with a **User** token to trigger a `403 Forbidden`, proving your Role-Based Access Control is working.


**4. Stress Test: Rate Limiting (Day 19)**

* **The Goal:** Prevent brute-force attacks.
* **Action:** Rapidly click "Send" on the `/login` endpoint.
* **Result:** After the 10th request within 10 seconds, the API will stop checking credentials and return `429 Too Many Requests`.

**5. Token Renewal**

* **Endpoint:** `POST /api/auth/refresh-token`
* **Action:** Send the request without a body. The API automatically reads the Refresh Token from your cookie and issues a **brand new** JWT.

---

### Final Portfolio Milestone

You’ve now documented a complete progression:

1. **SimpleTodo:** Data Persistence.
2. **IDVerification:** Performance & Mock Logic.
3. **AuthSystem:** Secure, Decoupled Architecture.

---