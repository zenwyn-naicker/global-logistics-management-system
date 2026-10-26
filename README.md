# Global Logistics Management System

A logistics platform for managing clients, freight contracts, signed agreements and service requests, with USD-to-ZAR cost conversion. It is split into an ASP.NET Core Web API secured with JWT and an ASP.NET Core MVC frontend that calls it, runs as a Docker Compose stack, and applies classic design patterns throughout the service layer.

Built for PROG7311 at Emeris Durban North (2026).

**Demo video:** [Watch on YouTube](https://youtu.be/5Z9Igpob7wA)

## Features

- **Clients and contracts:** client records with contact details and regions, linked to contracts with service levels and statuses (Draft, Active, Expired, On Hold, Declined).
- **Search and filtering:** filter contracts by date range and status.
- **Signed agreements:** upload a PDF agreement for a contract and download it later; other file types are rejected.
- **Service requests:** requests can only be raised against Active contracts, a rule enforced in the API's service layer.
- **Currency conversion:** USD costs are converted to ZAR using a live exchange-rate API, with cached and fixed-rate fallbacks and a preview before submitting.
- **REST API:** JSON endpoints with JWT bearer authentication and Swagger UI documentation.

## Design patterns

| Pattern | Use |
| --- | --- |
| Factory Method | Creates contracts in the correct initial status |
| Strategy | Switches between live, cached and fixed-rate currency conversion |
| Observer | Reacts to contract status changes with notification, audit and request-blocking observers |
| Repository | Separates data access from controllers and services |
| Service layer | Keeps business rules out of controllers |

## Tech stack

| Area | Technologies |
| --- | --- |
| Backend | C#, ASP.NET Core Web API (.NET 10), JWT bearer authentication, Swagger (Swashbuckle) |
| Frontend | ASP.NET Core MVC, Razor, Bootstrap, typed `HttpClient` |
| Data | SQL Server with Entity Framework Core 10 and migrations |
| Integration | ExchangeRate-API |
| Infrastructure | Docker, Docker Compose |
| Testing and CI | xUnit integration tests, GitHub Actions |

## Architecture

```text
GLMS.Web (MVC) --HTTP + JWT--> GLMS.Api (Web API) --EF Core--> SQL Server
                                   |-- Controllers
                                   |-- Services      contracts, service requests, agreements, currency
                                   |-- Factories, Strategies, Observers
                                   '-- Repositories
```

The MVC frontend never touches the database: it calls the API through a typed client, and the API owns the database connection and migrations. In Docker Compose, SQL Server, the API and the frontend run as separate containers on an internal network.

### Key endpoints

| Method | Endpoint | Purpose |
| --- | --- | --- |
| `POST` | `/api/auth/login` | Returns a JWT for the frontend |
| `GET` | `/api/health` | Health check |
| `GET`, `POST` | `/api/clients` | List and create clients |
| `GET`, `POST` | `/api/contracts` | List (with filters) and create contracts |
| `PATCH` | `/api/contracts/{id}/status` | Change a contract's status |
| `POST`, `GET` | `/api/contracts/{id}/agreement` | Upload and download a signed agreement |
| `GET`, `POST` | `/api/service-requests` | List and create service requests |
| `GET` | `/api/currency/preview` | Preview a USD-to-ZAR conversion |

## Getting started

### Run with Docker

1. Copy `.env.example` to `.env` and choose your own values. The JWT key needs at least 32 bytes, and SQL Server requires a strong password. The exchange-rate API key is optional because the app falls back to cached and fixed rates.
2. Start the stack:

```bash
docker compose up --build
```

3. Open the frontend at `http://localhost:8081` and Swagger UI at `http://localhost:8080/swagger`.

The SQL Server image needs a supported host architecture; on an Apple silicon Mac you may need an external SQL Server instance.

### Run with the .NET SDK

Requires the .NET 10 SDK and SQL Server.

1. In both `GLMS.Api` and `GLMS.Web`, copy `appsettings.Local.example.json` to `appsettings.Local.json` and fill in the connection string, JWT key and API credentials.
2. Build the solution, then start the API and the frontend in separate terminals:

```bash
dotnet build GLMS.slnx
```

```bash
dotnet run --project GLMS.Api
```

```bash
dotnet run --project GLMS.Web
```

The API applies its migrations on start-up. `.env` and `appsettings.Local.json` are ignored by Git.

## Testing

```bash
dotnet test GLMS.Tests
```

The integration tests run against a live API and cover the health check, authentication, and the full workflow from login through client and contract creation, status changes, currency preview, service requests and agreement upload and download. Set `GLMS_API_BASE_URL`, `GLMS_TEST_USERNAME` and `GLMS_TEST_PASSWORD` first, and use a disposable database. The GitHub Actions workflow builds the solution, starts SQL Server and the API, and runs the tests; it is set to run manually.

## Project structure

```text
GLMS.Api/     Web API: controllers, services, patterns, repositories, EF Core migrations
GLMS.Web/     MVC frontend and typed API client
GLMS.Tests/   xUnit integration tests
docker-compose.yml
```

## Acknowledgements

Documentation, course material and adapted code patterns are credited in [CODE_ATTRIBUTION.txt](CODE_ATTRIBUTION.txt).
