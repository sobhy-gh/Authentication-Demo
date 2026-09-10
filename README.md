# Authentication-Demo

# 🔐 Authentication & Authorization API

> A clean and secure Authentication system built with **ASP.NET Core .NET 10**

JWT • Refresh Tokens • Role-Based Authorization • EF Core

---

### ✨ What's inside?

A complete authentication flow that most real-world applications need:

- User Registration & Login
- JWT Access Tokens
- Refresh Tokens (stored in **HttpOnly Cookies**)
- Role-Based Authorization (`Admin` / `User`)
- Password Hashing
- User Management (CRUD)
- Global Exception Handling
- Rate Limiting
- Clean Architecture style structure

---

### 🛠 Tech Stack

| Technology              | Purpose                     |
|-------------------------|-----------------------------|
| ASP.NET Core .NET 10    | Web API                     |
| Entity Framework Core   | ORM                         |
| SQL Server              | Database                    |
| JWT                     | Access Tokens               |
| HttpOnly Cookies        | Refresh Tokens              |
| Scalar / OpenAPI        | API Documentation           |

---

### 🔄 Authentication Flow


Register → Login → Access Token + Refresh Token
         ↓
   Protected Endpoints
         ↓
   Access Token Expires
         ↓
   Use Refresh Token → Get New Access Token
         ↓
   Logout (Revoke Refresh Token)



## 📡 API Endpoints

### 🔐 Authentication

| Method | Endpoint             | Description            |
| ------ | -------------------- | ---------------------- |
| `POST` | `/api/auth/register` | Register a new user    |
| `POST` | `/api/auth/login`    | Login & receive tokens |
| `POST` | `/api/auth/refresh`  | Get a new Access Token |
| `POST` | `/api/auth/logout`   | Revoke Refresh Token   |

### 👤 Users

| Method   | Endpoint          | Access       |
| -------- | ----------------- | ------------ |
| `GET`    | `/api/users`      | Admin / User |
| `GET`    | `/api/users/{id}` | Admin        |
| `POST`   | `/api/users`      | Admin        |
| `PUT`    | `/api/users/{id}` | Admin        |
| `DELETE` | `/api/users/{id}` | Admin        |

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/sobhy-gh/Authentication-Demo.git
cd Authentication-Demo
```

### 2. Configure the Database

Update the connection string in `appsettings.json` according to your local SQL Server configuration.

### 3. Set JWT Secret

For local development, use **.NET User Secrets** instead of storing the secret in `appsettings.json`.

```bash
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Secret" "YOUR_SUPER_SECRET_KEY_HERE"
```

### 4. Apply Migrations

```bash
dotnet ef database update
```

### 5. Run the Project

```bash
dotnet run
```

Then open **Scalar** or **Swagger** to test the API endpoints.

---

## 🔒 Security Notes

* 🔑 JWT Secret is **not stored in `appsettings.json`**
* 🍪 Refresh Tokens are sent using **HttpOnly Cookies**
* 🔐 Passwords are securely hashed
* 🚦 Rate Limiting is enabled
* 🛡️ Role-Based Authorization is implemented
* 🔒 Sensitive data should be stored using **User Secrets** or **Environment Variables**

