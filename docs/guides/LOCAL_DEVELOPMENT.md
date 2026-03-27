# Sporting Club API - Local Development Guide

## ?? Quick Start

### Prerequisites
- .NET 7 SDK installed
- Windows/PowerShell or Linux/macOS with dotnet CLI

### Running Locally

#### Option 1: Batch Script (Windows)
```batch
.\run-api.bat
```

#### Option 2: Manual Command
```powershell
cd D:\Work\AI-Projects\Sporting-Club
dotnet run --project src/SportingClub.API
```

The API will start on: **http://localhost:5244**

---

## ?? Available Endpoints

### Authentication
- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/login` - Login user
- `POST /api/v1/auth/refresh` - Refresh access token
- `POST /api/v1/auth/logout` - Logout (requires Bearer token)
- `POST /api/v1/auth/forgot-password` - Request password reset
- `POST /api/v1/auth/reset-password` - Reset password

### Resources
- `GET /api/v1/resources` - List all resources
- `POST /api/v1/resources` - Create new resource
- `GET /api/v1/resources/{id}` - Get resource by ID
- `PUT /api/v1/resources/{id}` - Update resource
- `DELETE /api/v1/resources/{id}` - Delete resource

### Other Endpoints
- `GET /api/v1/analytics` - Analytics endpoints
- `GET /api/v1/files` - File management endpoints
- `GET /api/v1/notifications` - Notification endpoints
- `GET /api/v1/operations` - Operations endpoints
- `GET /api/v1/health` - Health check

---

## ?? Testing Endpoints

### Option 1: Swagger UI (Recommended)
Open in browser: **http://localhost:5244/swagger**

All endpoints are documented and can be tested directly from the UI.

### Option 2: PowerShell Test Script
```powershell
.\test-endpoints.ps1
```

This script will:
1. Test the health check
2. Register a new user
3. Login with the registered user
4. Display tokens and responses

### Option 3: Manual cURL / PowerShell

**Health Check:**
```powershell
Invoke-WebRequest -Uri "http://localhost:5244/api/v1/health" -UseBasicParsing
```

**Register User:**
```powershell
$body = @{
    email = "user@example.com"
    password = "Password@123"
    fullName = "John Doe"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5244/api/v1/auth/register" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body `
    -UseBasicParsing
```

**Login:**
```powershell
$body = @{
    email = "user@example.com"
    password = "Password@123"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5244/api/v1/auth/login" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body `
    -UseBasicParsing
```

---

## ?? Database

### Local Development (SQLite)
- **Type:** SQLite
- **File:** `bin/Debug/net7.0/sportingclub.db`
- **Auto-Migration:** Runs on startup

The database is automatically created and migrated when the API starts.

### Configuration
Edit `appsettings.json` to switch between SQLite (local) and PostgreSQL (production):

```json
{
  "UseLocalDb": true,  // Set to false for PostgreSQL
  "ConnectionStrings": {
    "Default": "Your_Connection_String_Here"
  }
}
```

---

## ?? Authentication

The API uses JWT (JSON Web Tokens) for authentication.

### Getting a Token

1. Register a user:
```json
POST /api/v1/auth/register
{
  "email": "user@example.com",
  "password": "Password@123",
  "fullName": "John Doe"
}
```

2. Login to get tokens:
```json
POST /api/v1/auth/login
{
  "email": "user@example.com",
  "password": "Password@123"
}

Response:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "550e8400-e29b-41d4...",
  "expiresIn": 3600
}
```

3. Use the access token in headers for authenticated requests:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## ?? Project Structure

```
src/
??? SportingClub.API/           # ASP.NET Core API (web layer)
?   ??? Controllers/            # API endpoints
?   ??? Program.cs              # App configuration
?   ??? appsettings.json        # Settings
?   ??? Properties/
?
??? SportingClub.Application/   # Application logic (CQRS, commands, queries)
?   ??? Services/               # Business logic services
?   ??? Interfaces/             # Service contracts
?
??? SportingClub.Infrastructure/ # Data access & external services
?   ??? SportingClubDbContext.cs # EF Core DbContext
?   ??? Migrations/             # Database migrations
?   ??? Services/               # Infrastructure implementations
?
??? SportingClub.Domain/        # Business entities & domain models
    ??? Models/                 # Domain entities
    ??? Interfaces/             # Domain interfaces
```

---

## ?? Troubleshooting

### API Won't Start
1. Check if port 5244 is already in use
2. Delete old database: `rm bin/Debug/net7.0/sportingclub.db`
3. Clean and rebuild: `dotnet clean && dotnet build`

### Database Errors
- Ensure EF Core migrations run successfully
- Check `appsettings.json` for correct `UseLocalDb` setting
- Clear bin/obj folders and rebuild

### Endpoint Returns 404
- Verify API is running on correct port (5244)
- Check route format: `/api/v1/auth/login` (not `/api/auth/login`)
- Visit Swagger UI to see all available endpoints

---

## ?? Development Notes

### API Versioning
The API uses URL-based versioning: `/api/v{version}`

Current version: **v1**

### Rate Limiting
- Applied to all endpoints except health check
- Limit: 100 requests per minute
- Returns 429 (Too Many Requests) when exceeded

### Audit Logging
- All requests are logged via `AuditLoggingMiddleware`
- Logs include: method, path, user email, timestamp

### Error Handling
The API returns standard HTTP status codes:
- `200` - Success
- `201` - Created
- `400` - Bad Request
- `401` - Unauthorized
- `403` - Forbidden
- `404` - Not Found
- `429` - Too Many Requests
- `500` - Server Error

---

## ?? Deployment

### Free Hosting Options

#### Option 1: Fly.io (Recommended)
```bash
flyctl auth login
flyctl launch --name sporting-club-api --no-deploy
flyctl secrets set UseLocalDb=false ConnectionStrings__Default="<your-db-connection>"
flyctl deploy
```

#### Option 2: Render.com
1. Connect GitHub repository
2. Create Web Service
3. Set environment variables
4. Deploy

#### Option 3: Railway.app
Similar to Render, with built-in PostgreSQL support

### Database for Production
Use one of these free PostgreSQL providers:
- **Neon** - Free tier with 3 projects
- **Supabase** - Free tier with 500MB storage
- **Railway** - Free tier included

---

## ?? Support

For issues or questions:
1. Check the Swagger UI documentation: http://localhost:5244/swagger
2. Review the troubleshooting section above
3. Check the GitHub repository for issues

---

**Happy coding! ??**
