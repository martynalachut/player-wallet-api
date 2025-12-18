# Player Wallet API

A simple **Player Wallet Web API** built with **ASP.NET Core**. The API allows creating wallet transactions (credit/debit), retrieving balances, and listing transaction history.

---

## 🧩 Features

* Create wallet transactions (credit / debit)
* Prevent overdrafts (balance cannot go below zero)
* Retrieve current player balance
* Retrieve transaction history per player
* In‑memory database using Entity Framework Core
* In‑memory caching for transaction reads
* Async implementation
* Swagger / OpenAPI documentation
* Unit testing

---

## 🚀 Running the Project

### Prerequisites

* .NET 7 or .NET 8 SDK

### Run the API

```bash
dotnet restore
dotnet run
```

The API will start at:

```
https://localhost:5001
```

### Swagger UI

Swagger is enabled in development mode:

```
https://localhost:5001/swagger
```

---

## 🔌 Database

The project uses **EF Core InMemory provider**:

```csharp
options.UseInMemoryDatabase("PlayerWalletDb");
```

This is suitable for:

* Development
* Demos
* Unit tests

⚠️ Data is not persisted between application restarts.

---

## 🧠 Caching Strategy

* Transaction lists are cached per `playerId` using `ConcurrentDictionary`
* Cache is **read‑through** and **invalidated on writes**

```csharp
ConcurrentDictionary<string, IReadOnlyList<PlayerTransactionDto>>
```

---

## 🧪 Testing

The project includes unit tests for all layers.

### Service Tests

* Mock the repository
* Validate business rules
* Test balance calculations
* Verify cache invalidation

### Repository Tests

* Use EF Core InMemory
* Verify persistence, filtering, and ordering
* No mocking of DbContext

### Controller Tests

* Mock the service layer
* Verify HTTP status codes
* Validate request handling and error mapping

Run all tests:

```bash
dotnet test
```

---

## 🔮 Future Improvements

The following enhancements would be natural next steps for a production system:

### Persistence

* Replace EF InMemory with:

  * Database e.g. SQL Server
  * EF Core migrations

### Caching

* Replace `ConcurrentDictionary` with:

  * `IMemoryCache` or
  * Redis (distributed cache)

### Error Handling

* Introduce custom domain exceptions
* Global exception‑handling middleware
* Consistent error response contracts

### Concurrency

* Handle concurrent debits safely
* Add optimistic concurrency (row versioning)

### API Design

* Return typed response DTOs everywhere
* Add pagination for transaction history
* Add idempotency keys for transaction creation

### Security

* Authentication / Authorization
* Rate limiting
* Request validation with FluentValidation

### Observability

* Structured logging
* Metrics

### Testing

* Integration tests with SQLite in‑memory
* End‑to‑end tests
