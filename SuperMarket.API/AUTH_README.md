# Authentication (Production-Grade)

## Overview

- **Roles**: `Admin`, `User`
- **Access token**: JWT, short-lived (default 15 min). Send in header: `Authorization: Bearer <accessToken>`
- **Refresh token**: Long-lived (default 7 days), stored in DB. Used to get a new access + refresh pair (rotation).

## Endpoints

| Method | Endpoint        | Auth    | Description                    |
|--------|-----------------|--------|--------------------------------|
| POST   | `/api/auth/login`   | None   | Login → access + refresh tokens |
| POST   | `/api/auth/refresh` | None   | Exchange refresh token for new tokens |
| POST   | `/api/auth/register`| Admin  | Register new user (Admin or User) |

All other APIs (`/api/categories`, `/api/customers`, `/api/products`, `/api/orders`) require **Admin or User** (valid access token).

## Login

**Request:** `POST /api/auth/login`

```json
{ "email": "admin@supermarket.local", "password": "Admin@123" }
```

**Response:**

```json
{
  "accessToken": "eyJhbG...",
  "refreshToken": "base64...",
  "accessTokenExpiresAtUtc": "2025-02-21T12:30:00Z"
}
```

## Refresh

**Request:** `POST /api/auth/refresh`

```json
{ "refreshToken": "<refresh_token_from_login>" }
```

**Response:** Same shape as login. Old refresh token is revoked; use the new one next time.

## Register (Admin only)

**Request:** `POST /api/auth/register`  
**Header:** `Authorization: Bearer <admin_access_token>`

```json
{ "email": "user@example.com", "password": "SecurePass8", "role": "User" }
```

`role` may be `"Admin"` or `"User"` (defaults to User if omitted).

## Config (`appsettings.json`)

```json
"Jwt": {
  "SecretKey": "AtLeast32CharactersLongSecretKey!",
  "Issuer": "SuperMarket.Api",
  "Audience": "SuperMarket.Client",
  "AccessTokenMinutes": 15,
  "RefreshTokenDays": 7
}
```

In production, set `Jwt:SecretKey` via environment or secrets (never commit real keys).

## Development seed

In **Development**, if no users exist, a default admin is created:

- **Email:** `admin@supermarket.local`
- **Password:** `Admin@123`

Change this password after first login in any real environment.

## Security features

- **Passwords**: BCrypt (work factor 12)
- **Refresh token rotation**: Each refresh issues a new pair and revokes the old token
- **JWT**: HMAC-SHA256, issuer/audience/lifetime validated, zero clock skew
- **Roles**: Enforced via `[Authorize(Roles = "Admin")]` / `[Authorize(Roles = "Admin,User")]`
