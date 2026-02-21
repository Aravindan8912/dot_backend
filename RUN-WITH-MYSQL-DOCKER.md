# Step-by-step: Run SuperMarket API with MySQL in Docker

Only **MySQL** runs in Docker. The **API** runs on your machine with `dotnet run`.

---

## Prerequisites

- **Docker Desktop** installed and running ([download](https://www.docker.com/products/docker-desktop/))
- **.NET 10 SDK** installed ([download](https://dotnet.microsoft.com/download))

---

## Step 1: Open the project folder

In PowerShell or Terminal:

```powershell
cd D:\DOT_Backend
```

(Use your actual path to the project.)

---

## Step 2: Start MySQL in Docker

Start the MySQL container:

```powershell
docker compose up -d
```

- Downloads the `mysql:8.0` image if needed
- Creates and starts container `supermarket-mysql`
- Exposes MySQL on **localhost:3307** (to avoid conflict with local MySQL on 3306)
- Creates database **SuperMarketDb**
- Root password: **root**

Wait until MySQL is ready (about 30–60 seconds on first run). Check status:

```powershell
docker compose ps
```

When the mysql service shows **Up (healthy)**, continue.

---

## Step 3: Run the API locally

In the **same** folder (`D:\DOT_Backend`):

```powershell
dotnet run --project SuperMarket.API
```

- Builds and runs the API
- API listens on **http://localhost:3000**
- Connects to MySQL at **localhost:3307** (from `appsettings.json`)

---

## Step 4: Test the API

- **Open in browser:** http://localhost:3000/openapi/v1.json  
- **Login (e.g. with curl):**

  ```powershell
  curl -X POST http://localhost:3000/api/auth/login -H "Content-Type: application/json" -d "{\"email\":\"admin@supermarket.local\",\"password\":\"Admin@123\"}"
  ```

  (In Development, the seeded admin is `admin@supermarket.local` / `Admin@123`.)

---

## Useful commands

| Action | Command |
|--------|--------|
| Start MySQL | `docker compose up -d` |
| Stop MySQL | `docker compose down` |
| Stop and delete DB data | `docker compose down -v` |
| View MySQL logs | `docker compose logs mysql` |
| Check if MySQL is running | `docker compose ps` |
| Run the API | `dotnet run --project SuperMarket.API` |

---

## If port 3306 is already in use

If you get **“port 3306 is not available”**:

1. In `docker-compose.yml`, change the MySQL ports line to:
   ```yaml
   ports:
     - "3307:3306"
   ```
2. In `SuperMarket.API/appsettings.json`, set the connection string to use port 3307:
   ```json
   "DefaultConnection": "Server=localhost;Port=3307;Database=SuperMarketDb;User Id=root;Password=root;"
   ```
3. Run `docker compose up -d` again and then run the API as in Step 3.

---

## Summary

1. `docker compose up -d` → MySQL runs in Docker (localhost:3307).
2. `dotnet run --project SuperMarket.API` → API runs on your machine (localhost:3000).
3. API uses `appsettings.json` and connects to **localhost:3307** MySQL.
