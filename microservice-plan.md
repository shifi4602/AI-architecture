# Microservice Decomposition Plan - WebApiShop

## Overview

Decompose the WebApiShop monolith into 4 independent microservices with synchronous HTTP
inter-service communication. The codebase has minimal cross-domain coupling - Orders already
use ID-only references, and the only true cross-domain dependency is FK validation on order creation.

---

## Service Boundaries

| Microservice          | Owns                 | Inbound Deps           | Outbound Deps                           |
|-----------------------|----------------------|------------------------|-----------------------------------------|
| ProductCatalog.API    | Product, Category    | Called by Orders.API   | None                                    |
| Identity.API          | User, ExistingUser   | Called by Orders.API   | None                                    |
| Orders.API            | Order, OrderItem     | None                   | Calls Identity.API & ProductCatalog.API |
| Auditing.API          | Rating               | Called by all services | None                                    |

---

## Language Selection

| Microservice        | Language            | Reasoning                                                                                          |
|---------------------|---------------------|----------------------------------------------------------------------------------------------------|
| ProductCatalog.API  | C# / ASP.NET Core   | Complex querying (filtering, sorting, pagination, .Include() joins) - EF Core excels here.         |
| Identity.API        | C# / ASP.NET Core   | Best-in-class JWT/auth libraries built into the framework. Mature and secure.                      |
| Orders.API          | C# / ASP.NET Core   | Transactional integrity (Order + OrderItems saved atomically). Typed HttpClient + Polly for calls. |
| Auditing.API        | Go or Node.js (TS)  | High-throughput, write-heavy, simple service. Lightweight runtime preferred. C# is also fine.      |

> **Note:** If the team only knows C#, use C# for all 4 services. Polyglot adds operational cost
> (CI/CD, monitoring, hiring). Only introduce a second language if the team is comfortable with it.

---

## Step-by-Step Plan

### Step 1 - Create 4 Independent ASP.NET Web API Projects

Each service gets its own solution with Controller -> Service -> Repository -> DbContext layers:

**ProductCatalog.API** - Owns Product + Category (kept together because ProductRepository does
`.Include(p => p.Category)`). Expose:
- `GET /api/products/{id}`       - for validation by Orders.API
- `GET /api/products?ids=1,2,3`  - batch endpoint for order hydration (avoids N+1)
- `GET /api/products`            - paginated, filtered, sorted search (existing logic)
- `GET /api/categories`          - list all categories

**Identity.API** - Owns User + ExistingUser. Expose:
- `POST /api/users`              - register
- `POST /api/users/login`        - login, returns JWT (replaces raw entity return)
- `GET  /api/users/{id}`         - get user by ID (for validation by Orders.API)
- `PUT  /api/users/{id}`         - update user
- `POST /api/passwords`          - password strength evaluation

**Orders.API** - Owns Order + OrderItem. Expose:
- `POST /api/orders`             - create order (validates User_Id and Product_Id via HTTP calls first)
- `GET  /api/orders/{id}`        - get order by ID

**Auditing.API** - Owns Rating. Expose:
- `POST /api/ratings`            - receive audit log entries from other services middleware

---

### Step 2 - Split the Single Database into 4 Databases

- Replace the single ApiShopContext (connection string "School") with per-service DbContexts.
- Each DbContext contains only its own DbSets.
- Remove cross-domain FK constraints:
  - Remove FK_Users_Orders      (Order.User_Id -> Users)
  - Remove FK_OrderItem_Products (OrderItem.Product_Id -> Products)
  - FK_Products_Categories stays (Product + Category are in the same service)
- Order/OrderItem tables keep User_Id and Product_Id as plain integer columns (no navigation properties).

---

### Step 3 - Add Typed HttpClient Services in Orders.API

- Register named/typed HttpClient instances in DI, pointing to Identity.API and ProductCatalog.API
  base URLs (configured in appsettings.json).
- In OrderService.CreateOrder():
  1. Call GET /api/users/{userId} on Identity.API       - return 400 if user does not exist.
  2. Call GET /api/products/{productId} on ProductCatalog.API for each OrderItem - return 400
     if any product does not exist.
  3. Only after both validations pass, save the Order + OrderItems to the database.
- This validation does NOT exist today - the monolith relied on database FK constraints.

---

### Step 4 - Add Batch Endpoint to ProductCatalog.API

- `GET /api/products?ids=1,2,3` - accepts a list of product IDs, returns matching products.
- Needed by Orders.API when fetching an order with multiple OrderItems, to hydrate product
  details (name, price) without making N+1 HTTP calls.

---

### Step 5 - Set Up JWT Authentication

- Identity.API issues a JWT on successful login:
  - Generate token with claims (userId, email, role).
  - Return token in login response (replaces current raw User entity return).
  - Hash passwords with BCrypt (current: plain text - critical security fix).
- All other services add AddAuthentication().AddJwtBearer() with the same signing key
  to validate incoming tokens and protect endpoints.

---

### Step 6 - Extract Per-Service Infrastructure

Each service gets:
- Its own DbContext with only its DbSets.
- Its own AutoMapper profile (split from the global one in UsersServicies/AutoMapper.cs).
- Its own copy of ErrorHandlingMiddleware.
- Its own appsettings.json with a dedicated connection string.
- A copy of RatingMiddleware that sends HTTP POST to Auditing.API instead of writing to DB directly.

---

## Cross-Cutting Concerns

### Resilience (Polly)
Add retry and circuit-breaker policies on HttpClient in Orders.API to handle transient failures
from Identity.API or ProductCatalog.API. Without this, one service going down cascades failures.

### API Gateway (Ocelot / YARP)
Recommended as a single entry point that routes:
- /api/products/* -> ProductCatalog.API
- /api/users/*    -> Identity.API
- /api/orders/*   -> Orders.API
- /api/ratings/*  -> Auditing.API

Handles CORS, centralizes JWT validation, and keeps client code unchanged.

### Auditing Reliability
Fire-and-forget HTTP to Auditing.API risks losing audit data if that service is down.
If audit completeness matters, consider a lightweight message queue (e.g., RabbitMQ) just
for this one concern, even while keeping everything else synchronous.

### Shared Contracts
Publish a small shared NuGet package with common DTOs (e.g., ProductValidationResponse,
UserValidationResponse) so the three C# services stay in sync without duplicating models.

---

## Cleanup Tasks

- Delete the orphaned DTO's/ project - not referenced by any project.
- Move ExistingUser and PassEntity out of the Enteties project into Identity.API.
- Fix password storage - add BCrypt hashing (currently plain text).
- Fix duplicate GET routes in UsersController (two GetUser methods cause runtime conflict).
- Remove the login endpoint raw User entity return (leaks password hash to client).

---

## Architecture Diagram

```
                     +------------------+
                     |   API Gateway    |
                     |  (Ocelot/YARP)   |
                     +--------+---------+
              +---------------+---------------+
              |               |               |
    +---------+--------+ +----+------+ +------+---------+
    | ProductCatalog   | | Identity  | |    Orders      |
    |     .API         | |   .API    | |     .API       |
    |  (C# / .NET)     | |(C# / .NET)| |  (C# / .NET)   |
    |                  | |           | |                |
    | Product          | | User      | | Order          |
    | Category         | | JWT Auth  | | OrderItem      |
    +--------+---------+ +-----+-----+ +---+--------+---+
             |                 |           |        |
             |   HTTP GET      | HTTP GET  |        |
             |<----------------+-----------+        |
             |                 |<-------------------+
    +--------+---------+ +-----+-----+ +------------+---+
    | ProductCatalog   | | Identity  | |    Orders      |
    |   Database       | | Database  | |   Database     |
    +------------------+ +-----------+ +----------------+

    All services send HTTP POST to:
    +------------------+
    |  Auditing.API    |
    | (Go/Node.js/C#)  |
    |  Rating          |
    +--------+---------+
    +--------+---------+
    |  Auditing        |
    |  Database        |
    +------------------+
```
