# How to check data in Postman

## 1. Base URL

Set **baseUrl** in the collection (or in an environment) to your API URL, for example:

- `https://localhost:5001` (HTTPS)
- `http://localhost:5000` (HTTP)

Use the URL shown when you run `dotnet run` or `dotnet watch run`.

---

## 2. GET = check / list data

Use **GET** requests to see what’s in the database.

| What you want to check | Method | URL |
|------------------------|--------|-----|
| All categories         | GET    | `{{baseUrl}}/api/categories` |
| One category           | GET    | `{{baseUrl}}/api/categories/{id}` |
| All customers          | GET    | `{{baseUrl}}/api/customers` |
| One customer           | GET    | `{{baseUrl}}/api/customers/{id}` |
| All products           | GET    | `{{baseUrl}}/api/products` |
| One product            | GET    | `{{baseUrl}}/api/products/{id}` |
| All orders             | GET    | `{{baseUrl}}/api/orders` |
| One order (with items) | GET    | `{{baseUrl}}/api/orders/{id}` |

Replace `{id}` with a real GUID (e.g. from a previous GET response).

---

## 3. Where to see the data in Postman

1. Send a GET request (e.g. **Get all products**).
2. In the **Response** section:
   - **Body** tab: JSON list or object (your data).
   - **Status**: e.g. `200 OK` if it worked.
3. To use an ID in another request: copy the `id` from the response and set the collection variable (e.g. `productId`, `categoryId`) or paste it in the URL.

---

## 4. Suggested order when testing

1. **GET** `api/categories` → see categories (or empty `[]`). You need at least one category to create products (e.g. add via DB/seed if no POST yet).
2. **GET** `api/customers` → see customers (or empty `[]`). You need at least one customer to create orders.
3. **POST** `api/products` → create a product (use a valid `categoryId` from step 1).
4. **GET** `api/products` → check that the product appears.
5. **POST** `api/orders` → create an order (use a valid `customerId` and `productId`).
6. **GET** `api/orders` → check that the order appears with its items.

---

## 5. Postman collection

Import **SuperMarket.API.postman_collection.json** (in this folder). It contains:

- **Categories**: Get all, Get by id  
- **Customers**: Get all, Get by id  
- **Products**: Get all, Get by id, Create, Update  
- **Orders**: Get all, Get by id, Create  

Use the **Get all** requests to check data; use **Create** with IDs from the GET responses.
