# DOT_Backend — Folder analysis (clear overview)

Quick reference for every folder and what it contains.

---

## Root

| Item | Purpose |
|------|--------|
| `SuperMarket.slnx` | Solution file — references all 4 projects |
| `README.md` | Main docs, API details, run instructions |
| `Dockerfile` | Builds and runs the API in a container |
| `docker-compose.yml` | Runs API + MySQL (and optionally Redis) |
| `.github/workflows/` | CI and CD pipelines |

---

## SuperMarket.API

**Role:** HTTP entry point. Controllers, request models, `Program.cs`, config.

| Folder/File | Contents |
|-------------|----------|
| `Program.cs` | App bootstrap, DI, middleware, OpenAPI |
| `appsettings.json` / `appsettings.Development.json` | Config (DB, JWT, etc.) |
| `Controllers/` | `AuthController`, `CategoriesController`, `CustomersController`, `OrdersController`, `ProductsController` |
| `Requests/` | API request models: `LoginRequest`, `RegisterRequest`, `RefreshTokenRequest`, `CreateCategoryRequest`, `CreateCustomerRequest`, `CreateOrderRequest`, `CreateProductRequest`, `UpdateProductRequest` |

**Depends on:** Application, Infrastructure (wired in `Program.cs`).

---

## SuperMarket.Application

**Role:** Use cases and application contracts. No DB or HTTP — only interfaces and DTOs.

| Folder | Contents |
|--------|----------|
| `DTOs/` | `CreateOrderItemDto`, `CreateOrdersDto`, `createProductDto`, `UpdateProductDto`, `UserLoginDto` |
| `Interfaces/` | Repository and service contracts: `ICategoryRepository`, `ICustomerRepository`, `IOrderRepository`, `IProductRepository`, `IRefreshTokenRepository`, `IUserRepository`, `IPasswordHasher`, `ITokenService` |
| `UseCases/` | Per-feature use cases: `Auth` (Login, RefreshToken, Register), `Categories` (CreateCategory), `Customers` (CreateCustomer), `Orders` (CreateOrder), `Products` (CreateProduct, UpdateProduct) |

**Depends on:** Domain only.

---

## SuperMarket.Domain

**Role:** Core business entities and enums. No dependencies on other projects.

| Folder | Contents |
|--------|----------|
| `Common/` | `BaseEntitiy.cs` — base class with `Id` (Guid) for entities |
| `Entities/` | `Category`, `Customer`, `Order`, `OrderItem`, `Product`, `RefreshToken`, `User` |
| `Enums/` | `OrderStatus`, `Role` |

**Depends on:** Nothing (shared by Application and Infrastructure).

---

## SuperMarket.Infrastructure

**Role:** Data access, EF Core, external services (JWT, password hashing).

| Folder/File | Contents |
|-------------|----------|
| `DependencyInjection.cs` | Registers DbContext, repositories, JWT, password hasher, etc. |
| `SeedData.cs` | Initial data seeding |
| `Migrations/` | EF Core migrations (InitialCreate, AddAuthAndRefreshTokens, AddCustomerUserId) |
| `Persistence/` | `AppDbContext`; `Configurations/` for Category, Customer, Order, OrderItem, Product, RefreshToken, User |
| `Repositories/` | Implementations: `CategoryRepository`, `CustomerRepository`, `OrderRepository`, `ProductRepository`, `RefreshTokenRepository`, `UserRepository` |
| `Services/` | `BCryptPasswordHasher`, `DateTimeService`, `JwtTokenService` |

**Depends on:** Domain, Application (implements interfaces).

---

## .github/workflows

| File | Purpose |
|------|--------|
| `ci.yml` | CI: build and test on push/PR |
| `cd.yml` | CD: deploy (e.g. after merge to main) |

---

## Dependency flow (clean architecture)

```
SuperMarket.API
    → SuperMarket.Application
    → SuperMarket.Infrastructure
        → SuperMarket.Domain
        → SuperMarket.Application (interfaces)
```

- **Domain:** center; no references to other projects.
- **Application:** references Domain; defines interfaces used by Infrastructure.
- **Infrastructure:** implements Application interfaces; uses Domain entities.
- **API:** orchestrates Application use cases and Infrastructure via DI.

---

## Cleanup / consistency notes

- **Remove:** `Class1.cs` in Domain, Application, and Infrastructure (leftover template files; not referenced).
- **Naming:** `BaseEntitiy.cs` has a typo (should be `BaseEntity.cs`); `createProductDto.cs` and `order.cs` use inconsistent casing vs rest of codebase — consider renaming for consistency.

---

*Generated as a clear folder overview for DOT_Backend.*
