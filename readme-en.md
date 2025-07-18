# 🇬🇧 Car Rental System - Backend (.NET 9)

This repository contains the backend of a car rental system developed with **.NET 9**, following **Clean Architecture**, **DDD**, and **CQRS** principles. It is designed to integrate with an Angular frontend and supports features like authentication, vehicle reservations, and service tracking.

> ⚠️ This repository was created for a technical evaluation and may be removed after the interview.

---

## ⚙️ Technologies & Architecture

* **.NET 9**, C# 12
* **Clean Architecture**, **DDD**, and **CQRS**
* **Entity Framework Core** with code-first migrations
* **JWT** authentication and role-based authorization
* **xUnit**, **FluentAssertions**, **NSubstitute**
* **OpenTelemetry**, HealthChecks, centralized logging
* **Swagger/OpenAPI** with XML docs and JWT support
* **Azure Durable Functions** (prepared for email workflows)

---

## 🚀 Getting Started

```bash
cd src/CarRental.API
dotnet ef database update
dotnet run
```

Access Swagger:

```
https://localhost:7263/swagger
https://localhost:7263/swagger/index.html
```

Access Scalar:

```
https://localhost:7263/scalar
https://localhost:7263/openapi/v1.json
```

Health and readiness:

```
https://localhost:7263/health
https://localhost:7263/alive
```

---

## 🗂️ Project Structure

```
/src
 ├── CarRental.API            → REST API Layer
 ├── CarRental.Core           → Port interfaces
 ├── CarRental.Domain         → Domain model
 ├── CarRental.Infrastructure → EF Core, Identity, Email, Logging
 └── CarRental.UseCases       → CQRS Handlers and DTOs
/tests
 ├── CarRental.Tests.Functional   → End-to-end tests
 ├── CarRental.Tests.Integration  → DB and repository tests
 └── CarRental.Tests.UseCases     → Unit tests for use cases
```

---

## 🧪 Testing Strategy

| Type          | Scope                       | Tools                         | Example                    |
| ------------- | --------------------------- | ----------------------------- | -------------------------- |
| ✅ Unit Tests  | Pure logic, services, rules | `xUnit`, `NSubstitute`        | CreateRentalCommandHandler |
| ✅ Integration | Repositories, real queries  | EF Core + SQLite/PostgreSQL   | EfRentalRepository         |
| ✅ E2E         | Full HTTP flow              | `TestServer`, `WebAppFactory` | Rental full flow           |

Run tests:

```bash
cd tests/CarRental.Tests.UseCases
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🔐 Authentication & Authorization

* User registration/login via JWT
* Token-based authentication with secure configuration
* ASP.NET Core Identity

  * User, role (`Admin`, `User`) and claims management with seeded data (`Seeder`)
* Role-based route protection: `[Authorize(Roles = "Admin")]`
* Public access: `[AllowAnonymous]`

### 📦 Database

* **EF Core (Entity Framework Core)** – Code-first ORM supporting multiple providers:

  * SQL Server
  * PostgreSQL
  * SQLite (used in GitHub Actions)

### 🧰 Validation & Behavior

* **FluentValidation** – Declarative validation for DTOs and commands
* **Validation Pipeline with MediatR** – Automatically validates before handling commands

### 🛠️ Services & Utilities

* **AutoMapper** – Maps between entities and DTOs
* **Serilog** – Structured logging configurable via `appsettings.json`
* **Swagger + Scalar** – Interactive API documentation:

  * `/swagger`, `/openapi/v1.json`, `/scalar`

### 📧 Email Services

> Inspired by the [`Clean.Architecture.Infrastructure`](https://github.com/ardalis/CleanArchitecture) project by Ardalis.

* Abstraction via `IEmailService`, interchangeable implementations:

  * `FakeEmailSender`, `MimeKitEmailSender`

---

## 📈 Observability

* ✅ Health endpoints: `/health`, `/alive`
* ✅ Tracing with OpenTelemetry (ASP.NET and HttpClient)
* ✅ Centralized logging with Serilog (console + file)

---

## 📦 EF Core Migrations

Create provider-specific migrations:

```bash
dotnet ef migrations add InitSQLite \
  --project CarRental.Infrastructure \
  --startup-project CarRental.API \
  --output-dir Migrations/SQLite

dotnet ef database update \
  --project CarRental.Infrastructure \
  --startup-project CarRental.API
```

---

# 🚗 CRUD API - Car Example

| Método HTTP | Ruta                  | Descripción                              | Código de Respuesta | Notas                            |
|-------------|-----------------------|------------------------------------------|----------------------|----------------------------------|
| POST        | /api/car              | Crea un nuevo auto                       | 201 Created          | Devuelve el ID del nuevo auto   |
| GET         | /api/car              | Lista todos los autos activos            | 200 OK               | Soporta paginación/filtrado     |
| GET         | /api/car/{id}         | Obtiene un auto por su ID                | 200 OK / 404 NotFound| Solo activos                    |
| PUT         | /api/car/{id}         | Actualiza un auto existente              | 204 NoContent / 400  | Requiere coincidencia de ID     |
| DELETE      | /api/car/{id}         | Elimina lógicamente un auto              | 204 NoContent / 404  | Marca como IsActive = false     |


## ✅ Functional Test Summary for `/api/car`

This table provides a professional overview of functional tests implemented in `CarApiFlowTests`. Each test follows consistent naming, emoji semantics, and verifies expected HTTP behaviors.

### 📋 CRUD & Flow Tests

| Emoji | Display Name                                           | Method     | Endpoint      | Expected Status | Notes                                            |
| ----- | ------------------------------------------------------ | ---------- | ------------- | --------------- | ------------------------------------------------ |
| ❤️    | should return 200 on /alive                            | GET        | /alive        | 200 OK          | Health check                                     |
| ✅     | Full car flow: create, duplicate, get all, fail on bad | Mixed      | /api/car      | 201, 400, 200   | Complete create-read-invalid scenario            |
| ✅     | Full car flow: create Tesla, verify, cleanup           | Mixed      | /api/car      | 201, 200        | Test with Tesla data                             |
| ➕     | should create a car (Toyota) successfully              | POST       | /api/car      | 201 Created     | Valid creation                                   |
| ➕     | should create car and return valid Id                  | POST       | /api/car      | 201 Created     | Check `CarId` validity                           |
| ⚫     | should retrieve car by ID                              | GET        | /api/car/{id} | 200 OK          | Retrieve specific car                            |
| ⚫     | should get all cars                                    | GET        | /api/car      | 200 OK          | List all                                         |
| ⚫     | should return only active cars                         | GET        | /api/car      | 200 OK          | Excludes logically deleted records               |
| 🚫    | should fail on duplicate car                           | POST       | /api/car      | 400 BadRequest  | Duplicate entry                                  |
| 🚫    | should fail on invalid car data                        | POST       | /api/car      | 400 BadRequest  | Validation error                                 |
| ❌     | should soft delete car                                 | DELETE     | /api/car/{id} | 204 NoContent   | Logical delete, no visible trace afterwards      |
| ❌     | should soft delete car and prevent future access       | DELETE/GET | /api/car/{id} | 204 / 404       | Tests access blocked after delete                |
| 🔁    | should fail to update car with outdated row version    | PUT        | /api/car/{id} | 204 / 409       | Concurrency (optimistic lock) with version check |

### 📌 Legend

* **➕** Create
* **⚫** Read/Get
* **🛠️** Update
* **❌** Delete
* **🚫** Failures (bad input, duplicates)
* **🔁** Concurrency conflict
* **✅** General success flow
* **❤️** Heartbeat or health endpoint

---


---

## 📘 Repository Method Naming Conventions

These method signatures represent standard naming conventions to improve clarity and traceability in the persistence layer:

| Method          | Description                               | Typical Return   | Notes                                         |
| --------------- | ----------------------------------------- | ---------------- | --------------------------------------------- |
| `GetByIdAsync`  | Retrieves an entity by its unique ID      | Entity or `null` | Direct, unique fetch                          |
| `FindByAsync`   | Finds an entity by one or more properties | Entity or `null` | Used for validations or single matches        |
| `ListAllAsync`  | Retrieves all entities of a type          | List of entities | Can include optional paging or projection     |
| `ListByAsync`   | Filters entities by property values       | List of entities | Returns 0 or more, depends on filter          |
| `ExistsByAsync` | Checks if entity exists under conditions  | `bool`           | Useful for uniqueness validations             |
| `AddAsync`      | Inserts a new entity into the repository  | `Task` / `void`  | Requires validation before adding             |
| `UpdateAsync`   | Updates an existing entity                | `Task` / `void`  | Can support optimistic concurrency            |
| `DeleteAsync`   | Deletes an entity (soft/hard)             | `Task` / `void`  | Often sets `IsActive = false` in soft deletes |

---

## 📘 Conventional Commits

We follow [Conventional Commits](https://www.conventionalcommits.org/) to keep the Git history semantic and automation-friendly:

| Type       | Purpose                           | Example                                                 |
| ---------- | --------------------------------- | ------------------------------------------------------- |
| `feat`     | New functionality                 | `feat(car): allow creation of car with version control` |
| `fix`      | Bug fix                           | `fix(car): increment version properly on soft delete`   |
| `refactor` | Internal code change, no behavior | `refactor(car): extract status logic into enum`         |
| `test`     | Adding or improving tests         | `test(car): add unit tests for CreateCarCommandHandler` |

---

## 📊 Test Coverage Report

### 🔧 Install Report Generator

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
```

### ▶️ Run Tests + Generate Coverage

**PowerShell:**

```powershell
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

**CMD:**

```cmd
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults && ^
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

📄 Generates coverage report at:

```
./coveragereport/index.html
```

---

## ⚙️ GitHub Actions: CI/CD

CI pipeline defined at:

```
.github/workflows/ci.yml
```

Includes:

* Automatic build
* Unit test validation
* Coverage support
* Integration test execution

---

**Author**: Martín Duhalde + ChatGPT (2025)
