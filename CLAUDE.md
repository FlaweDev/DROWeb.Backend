# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

DROWeb.Backend is a multi-layered ASP.NET 10.0 WebAPI for a Danganronpa fan game, following Clean Architecture:

```
DROWeb.Domain/        - Domain models (POCO entities)
DROWeb.Application/   - Application services and interfaces (MediatR)
DROWeb.Persistence/   - EF Core data access (PostgreSQL/SQLite)
DROWeb.WebAPI/        - Web API with FastEndpoints (minimal API)
DROWeb.Auth/          - OAuth providers (Discord)
DROWeb.UnitySDK/      - Unity client SDK (.NET Standard 2.1)
```

## Architecture

| Layer | Responsibility | Key Technologies |
|-------|----------------|------------------|
| Domain | Business entities only | POCO classes, events |
| Application | Service layer, DTOs, validation | MediatR, FluentValidation, Mapperly |
| Persistence | Data access, DbContext | EF Core 10.0, Npgsql, SQLite |
| WebAPI | HTTP endpoints | FastEndpoints 8.1.0 |
| Auth | External authentication | AspNet.Security.OAuth.Discord |

### Key Patterns
- **FastEndpoints** - attribute-based endpoint discovery (no routing attributes needed)
- **MediatR** - CQRS-like request/response pattern for domain events
- **Mapperly** - source-generated object mapping (no reflection)
- **Clean Architecture** - dependencies point inward (WebAPI → Application → Domain)

## Key Technologies

| Component | Technology | Version |
|-----------|------------|---------|
| Framework | ASP.NET Core | 10.0 |
| ORM | Entity Framework Core | 10.0.8 |
| Database (Prod) | PostgreSQL | via Npgsql |
| Database (Dev) | SQLite | in-memory/file |
| Auth | Discord OAuth | AspNet.Security.OAuth.Discord |
| API Framework | FastEndpoints | 8.1.0 |
| Validation | FluentValidation | 12.1.1 |
| Mapping | Mapperly | 4.3.1 |

## Build & Run

### Prerequisites
- .NET 10.0 SDK
- PostgreSQL server (for production) or SQLite (for development)

### Development

```bash
# Build all projects
dotnet build

# Run the WebAPI
dotnet run --project DROWeb.WebAPI

# Run tests (if added)
dotnet test
```

### Environment Variables

Create a `.env` file in `DROWeb.WebAPI/`:

```bash
# PostgreSQL (production)
DATABASE_HOST=localhost
DATABASE_PORT=5432
DATABASE_NAME=droweb
DATABASE_USER=your_user
DATABASE_PASSWORD=your_password

# Discord OAuth
CLIENT_ID=your_discord_client_id
CLIENT_SECRET=your_discord_client_secret

# Use SQLite for development
USE_SQLITE=true
DbConnection=Data Source=Players.db
```

**Note:** On Windows, use forward slashes in paths: `Data Source=C:/data/Players.db`

## Database

- Auto-created via `DbInitializer.Initialize()` on startup
- Uses `EnsureCreated()` - schema changes require manual migration
- Default SQLite path: `Players.db` in application directory

## API Endpoints

### Authentication
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/auth/login` | Start Discord OAuth | Anonymous |
| POST | `/api/auth/logout` | Sign out | Required |
| GET | `/api/auth/status` | Get current user status | Anonymous |

### Users
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/users/{Id}/avatar` | Get user avatar URL | Anonymous |

### Other
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/version` | Get API version | Anonymous |

### Static Pages (wwwroot)
- `/auth.html` - Login page with Discord OAuth button
- `/failed.html` - Error page for auth failures

## Authentication Flow

1. User visits `/auth.html`
2. Clicks "Войти через Discord" → redirects to `/api/auth/login`
3. OAuth flow redirects to Discord, then back to callback
4. `DiscordProvider.OnCreatingTicket()` creates/links user record
5. Cookie authentication stores user session
6. Frontend checks `/api/auth/status` to show authenticated UI

## Project-Specific Notes

### Adding New Endpoints
```csharp
// Use FastEndpoints pattern - no routing needed
public class MyEndpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/path");
        AllowAnonymous(); // or remove for authenticated
    }
    
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Handle request
    }
}
```

### Database Migrations
Since the project uses `EnsureCreated()` instead of migrations:
- For schema changes, manually update models
- Delete `Players.db` or drop PostgreSQL tables
- Restart app to recreate schema

### Windows Development Tips
- Use `forward/slashes` in file paths (works on Windows too)
- `.env` file is loaded via ` dotenv.Extensions` package
- PostgreSQL connection string uses `NpgsqlConnectionStringBuilder`
- The `USE_SQLITE` environment variable switches between DB providers

### Domain Events
Used via MediatR for decoupled business logic:
- `UserAuthenticated` - fired after successful OAuth
- Handlers process user creation/linking

## Troubleshooting

### Common Issues

**Discord auth not working:**
- Verify `CLIENT_ID` and `CLIENT_SECRET` in `.env`
- Check Discord OAuth redirect URI matches

**Database errors:**
- Ensure `DATABASE_HOST`, `DATABASE_PORT`, etc. are set
- Test PostgreSQL connection: `pg_isready -h localhost`

**CORS errors:**
- CORS enabled for all origins in Development
- Check `Startup.cs` Configure method
