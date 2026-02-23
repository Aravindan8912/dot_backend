# Postman — Full Admin Flow

Test the SuperMarket API as **Admin** in order: login → categories → customers → products → orders → addresses → payments. Use **http://localhost:3000** as base URL (or your configured URL).

---

## 1. Set base URL

In Postman, create an **Environment** (optional) or use a collection variable:

- **Variable:** `baseUrl` = `http://localhost:3000`

Then use `{{baseUrl}}` in request URLs, or type the full URL each time.

---

## 2. Login as Admin (get token)

**Request**

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/auth/login`
- **Headers:** `Content-Type: application/json`
- **Body (raw, JSON):**

```json
{
  "email": "admin@supermarket.local",
  "password": "Admin@123"
}
```

**Response (example)**

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123...",
  "accessTokenExpiresAtUtc": "2025-02-23T12:30:00Z"
}
```

**What to do:** Copy `accessToken`. For every request below (except login/refresh), add:

- **Header:** `Authorization` = `Bearer <paste accessToken here>`

In Postman you can also: **Authorization** tab → Type **Bearer Token** → paste the token once; it will be sent with all requests in the collection.

---

## 3. Create a category

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/categories`
- **Headers:** `Content-Type: application/json`, `Authorization: Bearer <accessToken>`
- **Body (raw, JSON):**

```json
{
  "name": "Beverages"
}
```

**Response:** You get the created category (with `id`). **Copy the category `id`** (GUID) for creating a product later.

---

## 4. Create a customer

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/customers`
- **Headers:** `Content-Type: application/json`, `Authorization: Bearer <accessToken>`
- **Body (raw, JSON):**

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "+1234567890"
}
```

**Response:** Created customer with `id`. **Copy the customer `id`** for creating an order and an address later.

---

## 5. Create a product

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/products`
- **Headers:** `Content-Type: application/json`, `Authorization: Bearer <accessToken>`
- **Body (raw, JSON):** Replace `categoryId` with the category id from step 3.

```json
{
  "name": "Mineral Water",
  "price": 1.99,
  "stock": 100,
  "categoryId": "PASTE_CATEGORY_ID_HERE"
}
```

**Response:** Created product with `id`. **Copy the product `id`** for creating an order.

---

## 6. Create an order

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/orders`
- **Headers:** `Content-Type: application/json`, `Authorization: Bearer <accessToken>`
- **Body (raw, JSON):** Replace `customerId` and `productId` with ids from steps 4 and 5.

```json
{
  "customerId": "PASTE_CUSTOMER_ID_HERE",
  "items": [
    {
      "productId": "PASTE_PRODUCT_ID_HERE",
      "quantity": 2
    }
  ]
}
```

**Response:** Created order with `id`. **Copy the order `id`** for creating a payment later.

---

## 7. Create an address (for the customer)

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/addresses`
- **Headers:** `Content-Type: application/json`, `Authorization: Bearer <accessToken>`
- **Body (raw, JSON):** Use the same `customerId` as in the order.

```json
{
  "customerId": "PASTE_CUSTOMER_ID_HERE",
  "addressLine1": "123 Main St",
  "addressLine2": "Apt 4",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "country": "USA"
}
```

**Response:** `{ "id": "...", "message": "Address created successfully" }`

---

## 8. Create a payment (for the order)

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/payments`
- **Headers:** `Content-Type: application/json`, `Authorization: Bearer <accessToken>`
- **Body (raw, JSON):** Use the order `id` from step 6. Amount can match order total or any value.

```json
{
  "orderId": "PASTE_ORDER_ID_HERE",
  "amount": 3.98,
  "paymentMethod": "Card"
}
```

**Response:** `{ "id": "...", "message": "Payment created successfully" }`

---

## 9. GET endpoints (verify data)

Use the same **Authorization: Bearer &lt;accessToken&gt;** header.

| Method | URL | Description |
|--------|-----|-------------|
| GET | `http://localhost:3000/api/categories` | List all categories |
| GET | `http://localhost:3000/api/customers` | List all customers |
| GET | `http://localhost:3000/api/products` | List all products |
| GET | `http://localhost:3000/api/orders` | List all orders |
| GET | `http://localhost:3000/api/addresses?customerId=<customerId>` | Addresses for a customer |
| GET | `http://localhost:3000/api/payments?orderId=<orderId>` | Payment for an order |

Replace `<customerId>` and `<orderId>` with the ids you used above.

---

## 10. Admin-only: Register a new user

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/auth/register`
- **Headers:** `Content-Type: application/json`, `Authorization: Bearer <accessToken>` (admin token)
- **Body (raw, JSON):**

```json
{
  "email": "user@example.com",
  "password": "User@456",
  "role": "User"
}
```

**Response:** `{ "id": "...", "email": "user@example.com", "role": "User" }`

---

## 11. Refresh token (when access token expires)

- **Method:** `POST`
- **URL:** `http://localhost:3000/api/auth/refresh`
- **Headers:** `Content-Type: application/json`
- **Body (raw, JSON):** Use the `refreshToken` from the login response.

```json
{
  "refreshToken": "PASTE_REFRESH_TOKEN_HERE"
}
```

**Response:** New `accessToken` and `refreshToken`. Use the new access token for subsequent requests.

---

## Quick checklist (order matters)

1. **POST** `/api/auth/login` → copy `accessToken`
2. Set **Authorization: Bearer &lt;accessToken&gt;** for all next requests
3. **POST** `/api/categories` → copy category `id`
4. **POST** `/api/customers` → copy customer `id`
5. **POST** `/api/products` (use category id) → copy product `id`
6. **POST** `/api/orders` (use customer id, product id in items) → copy order `id`
7. **POST** `/api/addresses` (use customer id)
8. **POST** `/api/payments` (use order id)
9. **GET** any of the above to verify

---

## Troubleshooting

- **401 Unauthorized:** Token missing or expired. Login again or use **POST /api/auth/refresh** with your refresh token.
- **400 Bad Request:** Check JSON body (property names, types). IDs must be valid GUIDs.
- **404 Not Found:** Wrong URL or wrong port. Default is `http://localhost:3000` (see `appsettings.json`).
- **Connection refused:** Start the API with `dotnet run --project SuperMarket.API` and ensure MySQL is running (e.g. `docker compose up -d`).
