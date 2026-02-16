# TaskManager API (.NET 8 + EF Core + PostgreSQL + JWT)

A production-ready RESTful API built with Clean Architecture principles for managing todo items with user authentication and authorization.

## Tech Stack

- **.NET 8** - Web API with minimal hosting model
- **Entity Framework Core 8** - PostgreSQL provider with migrations
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation pipeline
- **JWT Bearer Authentication** - Secure token-based auth
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - Interactive API documentation
- **BCrypt.Net** - Password hashing
- **PostgreSQL 14+** - Relational database

## Architecture

Clean Architecture with vertical slice organization:


## Key Features

### Domain Model
- **TodoItem**: Title, Description, Priority (Low/Medium/High), Status (Pending/InProgress/Completed), DueDate, soft delete
- **User**: Email (unique), password (BCrypt hashed), one-to-many with TodoItems
- **BaseEntity**: Id (Guid), CreatedAt, UpdatedAt (with concurrency token)

### Application Layer
- **CQRS Pattern**: Separate commands (write) and queries (read) with MediatR
- **Validation Pipeline**: Automatic request validation using FluentValidation behavior
- **Result Pattern**: Standardized error handling with `Result<T>` wrapper
- **Pagination**: Built-in paginated queries for todo lists

### Infrastructure
- **Repository Pattern**: Abstraction over EF Core with compiled queries
- **Soft Delete**: Global query filter automatically excludes deleted todos
- **Optimistic Concurrency**: `UpdatedAt` as concurrency token prevents lost updates
- **Database Indexes**: Composite indexes on `(UserId, Status)`, `(UserId, Priority)`, `DueDate`
- **Enum Conversion**: Priority/Status stored as strings for readability

### API Layer
- **JWT Authorization**: Attribute-based protection with `[Authorize]`
- **Current User Service**: Extracts user context from claims
- **Global Exception Handler**: Returns RFC 7807 ProblemDetails
- **Rate Limiting**: Fixed window (100 requests/minute)
- **CORS**: Configured for development
- **Health Checks**: `/health` endpoint
- **Swagger UI**: Bearer token support for testing

## Prerequisites

- **.NET SDK 8.0** or higher ([Download](https://dotnet.microsoft.com/download))
- **PostgreSQL 14+** running locally or remotely
- *Optional*: Visual Studio 2022, VS Code, or Rider

## Getting Started



### 1. Configure Database

Edit `src/TaskManager.API/appsettings.json` or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=taskmanager_db;Username=YOUR_USER;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Issuer": "TaskManager.Api",
    "Audience": "TaskManager.Client",
    "SecretKey": "your-super-secret-key-min-32-characters",
    "ExpiryMinutes": 60
  }
}
```

### 2. Apply Migrations

Migrations run automatically on startup, or manually:

```bash
dotnet tool restore
dotnet ef database update \
  --project src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj \
  --startup-project src/TaskManager.API/TaskManager.API.csproj
```

### 3. Run the Application

```bash
dotnet run --project src/TaskManager.API/TaskManager.API.csproj
```

Expected output:
```
[INF] Now listening on: http://localhost:5018
[INF] Application started. Press Ctrl+C to shut down.
[INF] Hosting environment: Development
```

### 4. Access Swagger UI

Open your browser: **http://localhost:5018/swagger**

## Run with Docker

### Prerequisites

- Docker Desktop (or Docker Engine + Compose)

### Start API + PostgreSQL

```bash
docker compose up --build
```

This starts:

- API at `http://localhost:5018`
- Swagger at `http://localhost:5018/swagger`
- PostgreSQL on `localhost:5432`

### Stop containers

```bash
docker compose down
```

To also remove the Postgres volume:

```bash
docker compose down -v
```

## API Endpoints

### Authentication (Public)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Create new user account |
| POST | `/api/auth/login` | Login and receive JWT token |

**Register Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Login Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-02-16T13:00:00Z"
}
```

### Todo Management (Protected - Requires JWT)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/todos` | Get paginated list with filters |
| GET | `/api/todos/{id}` | Get single todo by ID |
| POST | `/api/todos` | Create new todo |
| PUT | `/api/todos/{id}` | Update existing todo |
| DELETE | `/api/todos/{id}` | Soft delete todo |

**Query Parameters (GET /api/todos):**
- `status` - Filter by status (Pending, InProgress, Completed)
- `priority` - Filter by priority (Low, Medium, High)
- `pageNumber` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 100)

**Create/Update Request:**
```json
{
  "title": "Complete code review",
  "description": "Review PR #123",
  "priority": "High",
  "status": "InProgress",
  "dueDate": "2026-02-20T00:00:00Z"
}
```

## Testing with Swagger

1. Navigate to `/swagger`
2. Call `POST /api/auth/register` to create an account
3. Call `POST /api/auth/login` to get a token
4. Copy the `accessToken` from response
5. Click **Authorize** button (🔒 icon)
6. Enter: `Bearer {your-token-here}`
7. Click **Authorize** and close the dialog
8. Now you can call all `/api/todos` endpoints

## Development Notes

### Adding New Migrations

```bash
dotnet ef migrations add MigrationName \
  --project src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj \
  --startup-project src/TaskManager.API/TaskManager.API.csproj
```

### EF Core Performance Optimizations

- ✅ `AsNoTracking()` on all read queries
- ✅ Projection with `.Select()` to fetch only needed columns
- ✅ Compiled queries for frequently-used lookups
- ✅ Composite indexes for common filter combinations
- ✅ Query splitting for related data
- ✅ Global query filters for soft delete

### Logging Configuration

Serilog is configured via `appsettings.json`:
- Console output for all environments
- Minimum level: Information (Warning for Microsoft.* except Hosting.Lifetime)
- Structured JSON format available if needed

### Security Features

- ✅ Passwords hashed with BCrypt (work factor 12)
- ✅ JWT tokens with expiration
- ✅ HTTPS redirection (production)
- ✅ Rate limiting (100 req/min)
- ✅ CORS configured per environment
- ✅ SQL injection protection via parameterized queries
- ✅ Input validation with FluentValidation


## Project Decisions

### Why Clean Architecture?
- Clear separation of concerns
- Business logic independent of frameworks
- Easily testable
- Scalable for team growth

### Why CQRS with MediatR?
- Simplified command/query separation
- Built-in pipeline behaviors (validation, logging)
- Loose coupling between layers
- Easy to add cross-cutting concerns

### Why Repository Pattern?
- Abstracts data access
- Enables unit testing with mocks
- Centralizes query optimization
- Compiled queries for performance

### Why Soft Delete?
- Data recovery capability
- Audit trail preservation
- Better for production systems
- Transparent via global query filters


