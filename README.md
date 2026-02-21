# DOT Backend (SuperMarket API)

ASP.NET Core (.NET 10) backend for a SuperMarket application. Clean architecture with Domain, Application, Infrastructure, and API layers. JWT authentication with refresh-token rotation.

---

## File Structure

```
DOT_Backend/
├── SuperMarket.slnx
├── README.md
│
├── SuperMarket.API/
│   ├── Program.cs
│   ├── SuperMarket.API.csproj
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── CategoriesController.cs
│   │   ├── CustomersController.cs
│   │   ├── OrdersController.cs
│   │   └── ProductsController.cs
│   └── Requests/
│       ├── CreateCategoryRequest.cs
│       ├── CreateCustomerRequest.cs
│       ├── CreateOrderRequest.cs
│       ├── CreateProductRequest.cs
│       ├── LoginRequest.cs
│       ├── RefreshTokenRequest.cs
│       ├── RegisterRequest.cs
│       └── UpdateProductRequest.cs
│
├── SuperMarket.Application/
│   ├── SuperMarket.Application.csproj
│   ├── DTOs/
│   │   ├── CreateOrderItemDto.cs
│   │   ├── CreateOrdersDto.cs
│   │   ├── createProductDto.cs
│   │   ├── UpdateProductDto.cs
│   │   └── UserLoginDto.cs
│   ├── Interfaces/
│   │   ├── ICategoryRepository.cs
│   │   ├── ICustomerRepository.cs
│   │   ├── IOrderRepository.cs
│   │   ├── IPasswordHasher.cs
│   │   ├── IProductRepository.cs
│   │   ├── IRefreshTokenRepository.cs
│   │   ├── ITokenService.cs
│   │   └── IUserRepository.cs
│   └── UseCases/
│       ├── Auth/
│       │   ├── LoginUseCase.cs
│       │   ├── RefreshTokenUseCase.cs
│       │   └── RegisterUserUseCase.cs
│       ├── Categories/
│       │   └── CreateCategoryUseCase.cs
│       ├── Customers/
│       │   └── CreateCustomerUseCase.cs
│       ├── Orders/
│       │   └── CreateOrderUseCase.cs
│       └── Products/
│           ├── CreateProductUseCase.cs
│           └── UpdateProductUseCase.cs
│
├── SuperMarket.Domain/
│   ├── SuperMarket.Domain.csproj
│   ├── Common/
│   │   └── BaseEntitiy.cs
│   ├── Entities/
│   │   ├── Category.cs
│   │   ├── Customer.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── Product.cs
│   │   ├── RefreshToken.cs
│   │   └── User.cs
│   └── Enums/
│       ├── OrderStatus.cs
│       └── Role.cs
│
└── SuperMarket.Infrastructure/
    ├── SuperMarket.Infrastructure.csproj
    ├── DependencyInjection.cs
    ├── SeedData.cs
    ├── Migrations/
    │   ├── 20260221045820_InitialCreate.cs
    │   ├── 20260221055403_AddAuthAndRefreshTokens.cs
    │   └── 20260221060000_AddCustomerUserId.cs
    ├── Persistence/
    │   ├── AppDbContext.cs
    │   └── Configurations/
    │       ├── CategoryConfiguration.cs
    │       ├── CustomerConfiguration.cs
    │       ├── OrderConfiguration.cs
    │       ├── OrderItemConfiguration.cs
    │       ├── ProductConfiguration.cs
    │       ├── RefreshTokenConfiguration.cs
    │       └── UserConfiguration.cs
    ├── Repositories/
    │   ├── CategoryRepository.cs
    │   ├── CustomerRepository.cs
    │   ├── OrderRepository.cs
    │   ├── ProductRepository.cs
    │   ├── RefreshTokenRepository.cs
    │   └── UserRepository.cs
    └── Services/
        ├── BCryptPasswordHasher.cs
        ├── DateTimeService.cs
        └── JwtTokenService.cs
```

---

## API Details

Base path: `/api`. OpenAPI available at `/openapi/v1.json` when running.

### Authentication

- **JWT Bearer**: Protected endpoints require `Authorization: Bearer <accessToken>`.
- **Roles**: `Admin`, `User`. Register endpoint is Admin-only.

---

### Auth — `api/auth`

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/auth/login` | Anonymous | Login with email and password. Returns access and refresh tokens. |
| POST | `/api/auth/refresh` | Anonymous | Exchange a valid refresh token for new access + refresh tokens (rotation). |
| POST | `/api/auth/register` | Admin | Register a new user. Role: `"Admin"` or `"User"`. |

**Request / Response**

- **POST /api/auth/login**  
  Body: `{ "email": "string", "password": "string" }`  
  Response: `{ "accessToken": "string", "refreshToken": "string", "accessTokenExpiresAtUtc": "datetime" }`

- **POST /api/auth/refresh**  
  Body: `{ "refreshToken": "string" }`  
  Response: same as login.

- **POST /api/auth/register**  
  Body: `{ "email": "string", "password": "string", "role": "Admin" | "User" }`  
  Response: `{ "id": "guid", "email": "string", "role": "string" }`

---

### Categories — `api/categories`

All endpoints require **Admin or User**.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/categories` | Get all categories. |
| GET | `/api/categories/{id}` | Get category by GUID. |
| POST | `/api/categories` | Create a category. |

**Request**

- **POST /api/categories**  
  Body: `{ "name": "string" }`

---

### Customers — `api/customers`

All endpoints require **Admin or User**.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/customers` | Get all customers. |
| GET | `/api/customers/{id}` | Get customer by GUID. |
| POST | `/api/customers` | Create a customer. |

**Request**

- **POST /api/customers**  
  Body: `{ "name": "string", "email": "string", "phone": "string", "password": "string?", "role": "string?" }`

---

### Products — `api/products`

All endpoints require **Admin or User**.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/products` | Get all products. |
| GET | `/api/products/{id}` | Get product by GUID. |
| POST | `/api/products` | Create a product. |
| PUT | `/api/products/{id}` | Update a product. |

**Request**

- **POST /api/products**  
  Body: `{ "name": "string", "price": number, "stock": number, "categoryId": "guid" }`

- **PUT /api/products/{id}**  
  Body: `{ "name": "string", "price": number, "categoryId": "guid" }`

---

### Orders — `api/orders`

All endpoints require **Admin or User**.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/orders` | Get all orders. |
| GET | `/api/orders/{id}` | Get order by GUID. |
| POST | `/api/orders` | Create an order. |

**Request**

- **POST /api/orders**  
  Body: `{ "customerId": "guid", "items": [ { "productId": "guid", "quantity": number } ] }`

---

## Configuration

- **JWT**: Set `Jwt:SecretKey`, optionally `Jwt:Issuer` and `Jwt:Audience` in `appsettings.json` or environment.
- **Database**: Connection string is configured in Infrastructure (e.g. via `AddInfrastructure` in `Program.cs`).
- **Seed**: In Development, a default admin user is seeded if none exists (see `SeedData.cs`).

---

## How to Run This Project (Full Dependency Guide)

### 1. Prerequisites

Install the following on your machine:

| Dependency | Version | Purpose |
|------------|---------|---------|
| **.NET SDK** | **10.0** or later | Build and run the API. [Download](https://dotnet.microsoft.com/download) |
| **MySQL** | 8.x recommended | Database. [Download](https://dev.mysql.com/downloads/) or use [Docker](https://hub.docker.com/_/mysql) |

**Verify .NET:**

```bash
dotnet --version
```

You should see `10.x.x` or higher.

**Verify MySQL (optional):**

```bash
mysql --version
```

Or ensure MySQL is running (e.g. as a Windows service or Docker container).

---

### 2. Clone and Open the Repository

```bash
git clone <your-repo-url>
cd DOT_Backend
```

Open the solution in your IDE (e.g. Visual Studio, Rider, or VS Code with C# extension):

- Solution file: `SuperMarket.slnx` (or open the folder and set `SuperMarket.API` as startup project).

---

### 3. Restore Dependencies

From the **repository root** (`DOT_Backend`):

```bash
dotnet restore
```

This restores NuGet packages for all projects:

- **SuperMarket.API**: `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.OpenApi`, `Microsoft.EntityFrameworkCore.Design`
- **SuperMarket.Infrastructure**: `Microsoft.EntityFrameworkCore`, `Pomelo.EntityFrameworkCore.MySql`, `BCrypt.Net-Next`, `Microsoft.IdentityModel.Tokens`, `System.IdentityModel.Tokens.Jwt`, etc.
- **SuperMarket.Application** and **SuperMarket.Domain**: no extra packages (project references only).

---

### 4. Database Setup

The app uses **MySQL** with Entity Framework Core and **Pomelo.EntityFrameworkCore.MySql**. Migrations run automatically on startup.

**Option A — Local MySQL**

1. Install and start MySQL (e.g. on `localhost`, port `3306`).
2. Create a database (optional; migrations can create it if the user has permission):

   ```sql
   CREATE DATABASE IF NOT EXISTS SuperMarketDb;
   ```

3. Ensure the connection string in `SuperMarket.API/appsettings.json` (or `appsettings.Development.json`) matches your server:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=SuperMarketDb;User Id=root;Password=root;"
   }
   ```

   Adjust `Server`, `User Id`, and `Password` as needed.

**Option B — Docker MySQL**

```bash
docker run -d --name supermarket-mysql -e MYSQL_ROOT_PASSWORD=root -e MYSQL_DATABASE=SuperMarketDb -p 3306:3306 mysql:8
```

Then use the same connection string as above (or set it via [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) / environment variables).

**Run migrations**

Migrations are applied automatically when the API starts (`db.Database.Migrate()` in `Program.cs`). No manual step is required for a normal run.

To add a new migration (from repo root):

```bash
cd SuperMarket.Infrastructure
dotnet ef migrations add YourMigrationName --startup-project ../SuperMarket.API
```

---

### 5. Configuration (Optional Overrides)

Default config is in `SuperMarket.API/appsettings.json`:

| Setting | Default | Description |
|--------|---------|-------------|
| `ConnectionStrings:DefaultConnection` | `Server=localhost;Database=SuperMarketDb;User Id=root;Password=root;` | MySQL connection |
| `Jwt:SecretKey` | (see file) | Must be **≥ 32 characters** for HMAC signing |
| `Jwt:Issuer` | `SuperMarket.Api` | Token issuer |
| `Jwt:Audience` | `SuperMarket.Client` | Token audience |
| `Jwt:AccessTokenMinutes` | `15` | Access token lifetime |
| `Jwt:RefreshTokenDays` | `7` | Refresh token lifetime |
| `Kestrel:Endpoints:Http:Url` | `http://localhost:3000` | API base URL |

For local secrets, use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
cd SuperMarket.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=SuperMarketDb;User Id=myuser;Password=mypass;"
dotnet user-secrets set "Jwt:SecretKey" "YourSecretKeyAtLeast32CharactersLong!!"
```

---

### 6. Build and Run

**Build the solution:**

```bash
dotnet build
```

**Run the API:**

From repo root:

```bash
dotnet run --project SuperMarket.API
```

Or from the API project:

```bash
cd SuperMarket.API
dotnet run
```

The API will:

1. Apply pending EF Core migrations to the database.
2. In **Development**, seed a default admin user if none exists (see below).
3. Listen on **http://localhost:3000** (unless overridden in config).

**Open in browser:**

- API base: **http://localhost:3000**
- OpenAPI spec: **http://localhost:3000/openapi/v1.json**

---

### 7. First Login (Development Seed)

When running in **Development**, the first run seeds an admin user if the table is empty:

| Email | Password |
|-------|----------|
| `admin@supermarket.local` | `Admin@123` |

Example login:

```bash
curl -X POST http://localhost:3000/api/auth/login -H "Content-Type: application/json" -d "{\"email\":\"admin@supermarket.local\",\"password\":\"Admin@123\"}"
```

Use the returned `accessToken` in the `Authorization` header for protected endpoints:

```text
Authorization: Bearer <accessToken>
```

---

### 8. Quick Reference Commands

| Action | Command |
|--------|--------|
| Restore packages | `dotnet restore` |
| Build | `dotnet build` |
| Run API | `dotnet run --project SuperMarket.API` |
| Run in watch mode | `dotnet watch run --project SuperMarket.API` |
| Add EF migration | `cd SuperMarket.Infrastructure` then `dotnet ef migrations add <Name> --startup-project ../SuperMarket.API` |
| Update database from code | Automatic on app startup |

---

### Troubleshooting

- **"Jwt:SecretKey is required"** — Set `Jwt:SecretKey` in `appsettings.json` or User Secrets (min 32 characters).
- **MySQL connection refused** — Ensure MySQL is running and `ConnectionStrings:DefaultConnection` matches your server (host, port, user, password).
- **401 Unauthorized** — Use a valid `Authorization: Bearer <accessToken>` header; refresh with `POST /api/auth/refresh` if the token expired.
- **.NET 10 not found** — Install [.NET 10 SDK](https://dotnet.microsoft.com/download); confirm with `dotnet --version`.
