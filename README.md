# godot-multiplayer-realm-api

API backend for the **Godot Multiplayer Realm Engine**.

This repository provides authentication and account management services for the Godot multiplayer architecture:

Client → Realm → Zone

The API currently handles:

- Account registration
- Login
- JWT token issuance
- Database health checks

It is designed to integrate with the `godot-multiplayer-realm-engine` project.

---

# Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL (Npgsql)
- BCrypt password hashing
- JWT Bearer authentication

---

# Project Structure

RealmAuthApi/
- Controllers/
  - AuthController.cs
  - HealthController.cs
- Data/
  - RealmAuthDbContext.cs
- Models/
  - Account.cs
- Program.cs
- appsettings.json

---

# Endpoints

Base route:

```
http://localhost:<PORT>/api
```

---

## POST /api/auth/register

Registers a new account and returns a JWT.

### Request Body

```json
{
  "username": "Zach",
  "email": "zach@example.com",
  "password": "super_secure_password"
}
```

### Response

```json
{
  "accountId": 1,
  "username": "Zach",
  "token": "<jwt>"
}
```

### Validation Rules

- Username: 3–32 characters
- Email: must contain "@"
- Password: minimum 8 characters

---

## POST /api/auth/login

Logs in with username OR email and returns a JWT.

### Request Body

```json
{
  "usernameOrEmail": "Zach",
  "password": "super_secure_password"
}
```

### Response

```json
{
  "accountId": 1,
  "username": "Zach",
  "token": "<jwt>"
}
```

---

## GET /api/health/db

Checks if the API can connect to PostgreSQL.

### Response

```json
{
  "ok": true
}
```

---

# Configuration

Configuration lives in:

```
RealmAuthApi/appsettings.json
```

---

## PostgreSQL

Default connection:

- Host: localhost
- Port: 5433
- Database: realm_auth
- Username: realm_user
- Password: realm_password_change_me

Modify:

```json
"ConnectionStrings": {
  "RealmAuthDb": "Host=localhost;Port=5433;Database=realm_auth;Username=realm_user;Password=realm_password_change_me"
}
```

---

## JWT Settings

In `appsettings.json`:

```json
"Jwt": {
  "Issuer": "RealmAuthApi",
  "Audience": "RealmAuthApiClient",
  "Key": "CHANGE_THIS_TO_A_LONG_RANDOM_SECRET"
}
```

IMPORTANT:
- Use a long random secret (32+ characters)
- Never commit production secrets

---

# Quick Start (Local Development)

## 1. Start PostgreSQL (Docker Example)

```bash
docker run --name realm-auth-db \
  -e POSTGRES_DB=realm_auth \
  -e POSTGRES_USER=realm_user \
  -e POSTGRES_PASSWORD=realm_password_change_me \
  -p 5433:5432 \
  -d postgres:16
```

---

## 2. Apply EF Core Migrations

From inside the `RealmAuthApi` directory:

```bash
dotnet ef database update
```

If migrations do not exist yet:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 3. Run the API

```bash
dotnet run
```

You should see something like:

```
Now listening on: http://localhost:5000
```

---

## 4. Test With Swagger

Navigate to:

```
http://localhost:<PORT>/swagger
```

You can register and login directly from the Swagger UI.

---

# How This Fits Into The Realm Architecture

Current multiplayer structure:

1. Client connects to Realm server
2. Realm server handles zone orchestration
3. API handles account authentication

Future expansions may include:

- Character creation
- Character persistence
- Inventory storage
- Account progression
- Realm shard metadata
- Instance tracking
- Admin controls

---

# Production Notes

Before deploying:

- Replace JWT secret with a secure environment variable
- Move connection strings to environment variables
- Enable HTTPS
- Configure proper CORS policy
- Add rate limiting
- Add logging + monitoring
- Add refresh tokens if long-lived sessions are needed

---

# License

Personal project – not licensed for redistribution (yet).

---

Built to support a server-authoritative multiplayer ARPG architecture in Godot.
