# WebApiShop

A production-style ASP.NET Core Web API for an e-commerce shop, built with a clean layered architecture. The solution integrates SQL Server, Redis caching, Apache Kafka event streaming, JWT authentication, and Docker for infrastructure services.

---

## Table of Contents

- [Architecture](#architecture)
- [Solution Structure](#solution-structure)
- [Technologies](#technologies)
- [Domain Model](#domain-model)
- [API Endpoints](#api-endpoints)
- [Authentication & Authorization](#authentication--authorization)
- [Middleware](#middleware)
- [Caching](#caching)
- [Kafka Event Streaming](#kafka-event-streaming)
- [Rate Limiting](#rate-limiting)
- [Configuration](#configuration)
- [Running the Infrastructure (Docker)](#running-the-infrastructure-docker)
- [Running the Application](#running-the-application)
- [Running Tests](#running-tests)

---

## Architecture

The solution follows a layered (N-Tier) architecture with clear separation of concerns:

```
┌──────────────────────────────────────┐
│           WebApiShop (API)           │  ← Controllers, Middleware, DI wiring
├──────────────────────────────────────┤
│           Servicies (BLL)            │  ← Business logic, AutoMapper, Kafka producer
├──────────────────────────────────────┤
│         Repositories (DAL)           │  ← EF Core, SQL Server, repository pattern
├──────────────────────────────────────┤
│      Enteties / DTO_s (Models)       │  ← EF entities, DTOs
├──────────────────────────────────────┤
│    LogConsumerServer (Worker)        │  ← Background Kafka consumer service
└──────────────────────────────────────┘
```

---

## Solution Structure

| Project | Description |
|---|---|
| `WebApiShop` | Main ASP.NET Core Web API – controllers, middleware pipeline, DI registration |
| `Enteties` | EF Core entity classes (auto-generated via EF Core Power Tools) |
| `DTO_s` | Data Transfer Objects used across the API layer |
| `Repositories` | Repository interfaces and EF Core implementations; `ApiShopContext` (DbContext) |
| `Servicies` | Service interfaces and implementations; AutoMapper profiles; Kafka producer |
| `LogConsumerServer` | .NET Worker Service that consumes Kafka messages and logs order events |
| `TestProject` | xUnit integration and unit tests for repositories and services |

---

## Technologies

| Technology | Purpose |
|---|---|
| ASP.NET Core (.NET) | Web API framework |
| Entity Framework Core | ORM / SQL Server data access |
| SQL Server | Primary relational database |
| Redis (StackExchange.Redis) | Distributed cache for product queries |
| Apache Kafka (Confluent.Kafka) | Event streaming – order-created events |
| Zookeeper | Kafka coordination (via Docker) |
| Kafka UI (provectuslabs) | Browser-based Kafka management UI |
| JWT Bearer Authentication | Stateless authentication |
| AutoMapper | Object-to-object mapping |
| NLog | Structured logging |
| Swagger / OpenAPI | Interactive API documentation |
| Docker Compose | Local infrastructure orchestration |
| xUnit | Unit and integration testing |

---

## Domain Model

```
User ────── Order ────── OrderItem ────── Product ────── Category
                                           │
                                         Rating
```

| Entity | Key Fields |
|---|---|
| `User` | Id, Email, Password (hashed), FirstName, LastName, Role |
| `Order` | OrderId, OrderDate, OrderSum, UserId |
| `OrderItem` | relates Order ↔ Product |
| `Product` | ProductsId, ProductName, Price, CategoryId, Description |
| `Category` | CategoryId, CategoryName |
| `Rating` | Host, Method, Path, Referer, UserAgent, RecordDate |

---

## API Endpoints

All endpoints require a valid **JWT Bearer token** unless marked `[AllowAnonymous]`.

### Users – `/api/Users`

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/Users` | Anonymous | Register a new user. Returns JWT cookie + user DTO |
| `POST` | `/api/Users/login` | Anonymous | Login. Returns JWT cookie + user DTO |
| `PUT` | `/api/Users/{id}` | Required | Update user profile |
| `GET` | `/api/Users/{id}` | Required | Get user by ID |

### Products – `/api/Products`

| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/Products` | Anonymous | Paginated & filtered product list (cached in Redis) |
| `GET` | `/api/Products/{id}` | Anonymous | Get product by ID (cached in Redis) |
| `POST` | `/api/Products` | Required | Add a new product |
| `PUT` | `/api/Products/{id}` | Required | Update a product (invalidates cache) |
| `DELETE` | `/api/Products/{id}` | Required | Delete a product |

**Query parameters for `GET /api/Products`:**  
`position`, `skip`, `name`, `categoryIds[]`, `description`, `minPrice`, `maxPrice`, `orderBy`

### Orders – `/api/Orders`

| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/Orders/{id}` | Required | Get order by ID |
| `POST` | `/api/Orders` | Required | Create a new order (validates sum, publishes Kafka event) |

### Categories – `/api/Categories`

| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/Categories` | Anonymous | Get all categories |

### Passwords – `/api/Passwords`

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/Passwords/CheckPasswordStrength` | Anonymous | Evaluate password strength |

---

## Authentication & Authorization

- **JWT Bearer tokens** are issued on registration and login.
- Tokens are also written as **HttpOnly, Secure, SameSite=Strict cookies** (`jwt`) for browser clients.
- On each request the middleware checks the `Authorization` header first; if absent it reads the `jwt` cookie.
- Two roles are defined: `Admin` and `User` (see `AppRoles.cs`).

**Token configuration** (`appsettings.json`):
```json
"Jwt": {
  "Key": "<secret>",
  "Issuer": "WebApiShop",
  "Audience": "WebApiShopUsers",
  "ExpiresInMinutes": 60
}
```

---

## Middleware

The request pipeline (in order) includes:

| Middleware | Purpose |
|---|---|
| `ErrorHandlingMiddleware` | Global exception handler – returns 500 and logs stack trace |
| `RateLimitingMiddleware` | Fixed-window rate limiter (10 req / 10 s, queue limit 2) |
| `RatingMiddleware` | Records every request (host, method, path, user-agent) into the `Rating` table |
| `AuthenticationMiddleware` | JWT validation (built-in ASP.NET Core) |
| `AuthorizationMiddleware` | Role/policy enforcement (built-in ASP.NET Core) |

---

## Caching

Redis is used as a distributed cache for the Products endpoints.

- **List queries** are cached with a composite key built from all query parameters.
- **Single product** is cached under the key `product:id={id}`.
- Default TTL: **4 minutes** (configurable via `RedisCache:DefaultTtlMinutes`).
- Cache is invalidated on `PUT` / `DELETE` product operations.

```json
"RedisCache": {
  "ConnectionString": "localhost:6379,password=<password>,abortConnect=false",
  "DefaultTtlMinutes": 4
}
```

---

## Kafka Event Streaming

When a new **Order** is successfully created the `OrderService` publishes a message to Kafka.

### Producer (`KafkaProducerService`)
- Registered as a **singleton** in `WebApiShop`.
- Uses `Acks = All` for durability.
- Topic: configurable via `Kafka:Topic` (default: `order-created-topic`).

### Consumer (`LogConsumerServer`)
- Standalone **.NET Worker Service** (`BackgroundService`).
- Subscribes to the same Kafka topic and logs every consumed message.
- Group ID, topic, and bootstrap servers are configured in `LogConsumerServer/appsettings.json`.
- Run independently: `dotnet run --project LogConsumerServer`

```json
"Kafka": {
  "BootstrapServers": "localhost:9092",
  "Topic": "order-created-topic"
}
```

---

## Rate Limiting

Fixed-window rate limiter applied globally to all controller routes:

| Setting | Value |
|---|---|
| Policy name | `fixed` |
| Permit limit | 10 requests |
| Window | 10 seconds |
| Queue limit | 2 |
| Rejection code | `429 Too Many Requests` |

---

## Configuration

Key `appsettings.json` sections:

```json
{
  "ConnectionStrings": {
    "Home": "Data Source=<server>;Initial Catalog=215601303_ApiShop;Integrated Security=True;Trust Server Certificate=True;"
  },
  "RedisCache": {
    "ConnectionString": "localhost:6379,password=<password>,abortct=false",
    "DefaultTtlMinutes": 4
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "Topic": "order-created-topic"
  },
  "Jwt": {
    "Key": "<base64-secret>",
    "Issuer": "WebApiShop",
    "Audience": "WebApiShopUsers",
    "ExpiresInMinutes": 60
  }
}
```

> **Security note:** Never commit real secrets. Move `Jwt:Key`, Redis password, and connection strings to environment variables or a secrets manager in production.

---

## Running the Infrastructure (Docker)

The `docker-compose.yml` spins up:

| Service | Port | Description |
|---|---|---|
| `redis` | 6379 | Redis cache (password-protected) |
| `zookeeper` | 2181 | Kafka coordination |
| `kafka` | 9092 | Apache Kafka broker |
| `kafka-ui` | 8080 | Kafka UI – browse topics & consumer groups |

```bash
docker compose up -d
```

Kafka UI is accessible at **http://localhost:8080**.  
Swagger UI (when the API is running) is at **http://localhost:5140/swagger**.

---

## Running the Application

**Prerequisites:** .NET SDK, SQL Server, Docker

1. Start infrastructure:
   ```bash
   docker compose up -d
   ```

2. Apply EF Core migrations (first time):
   ```bash
   dotnet ef database update --project Repositories --startup-project WebApiShop
   ```

3. Run the main API:
   ```bash
   dotnet run --project WebApiShop
   ```

4. *(Optional)* Run the Kafka consumer:
   ```bash
   dotnet run --project LogConsumerServer
   ```

---

## Running Tests

```bash
dotnet test TestProject
```

The test project contains both **unit tests** and **integration tests** for:

| Test file | Coverage |
|---|---|
| `UserRepoUnitTest.cs` | User repository (mocked) |
| `UserRepositoryIntegrationTests.cs` | User repository against a real DB |
| `ProductReposoryUnitTest.cs` | Product repository (mocked) |
| `productRepositoryIntegrationTest.cs` | Product repository integration |
| `CategoryRepositoryTests.cs` | Category repository unit tests |
| `CategoryRepositoryIntegrationTest.cs` | Category repository integration |
| `OrdersRepositoryUnitTests.cs` | Orders repository (mocked) |
| `OrdersRepositoryIntegrationTests.cs` | Orders repository integration |
| `PasswordServicesTests.cs` | Password strength service |

Integration tests use `DatabaseFixture.cs` for shared DB setup/teardown.
