# 🇬🇧 Car Rental System - Backend (.NET 9)

This repository contains the backend of a car rental system built with **.NET 9**, following **Clean Architecture**, **DDD**, and **CQRS** principles. It is designed to integrate with an Angular frontend and supports features such as user authentication, vehicle reservation, and service tracking.

> ⚠️ This repository was created for a technical evaluation and may be removed after the interview.

---

## ⚙️ Technologies & Architecture

- **.NET 9**, C# 12
- **Clean Architecture** + **DDD** + **CQRS**
- **Entity Framework Core** with code-first migrations
- **JWT** authentication and **role-based** authorization
- **xUnit**, **FluentAssertions**, **NSubstitute**
- **OpenTelemetry**, HealthChecks, centralized logging
- **Swagger/OpenAPI** with XML docs and JWT support
- **Azure Durable Functions** (prepared for email workflows)

---

## 🚀 Getting Started

```bash
cd src/CarRental.API
dotnet ef database update
dotnet run
```

Access Swagger UI:

```
https://localhost:7263/swagger
https://localhost:7263/swagger/index.html
```

Access Scalar UI:

```
https://localhost:7263/scalar
https://localhost:7263/openapi/v1.json
```

Health & readiness:

```
https://localhost:7263/health
https://localhost:7263/alive
```

---

## 🗂️ Project Structure

```
/src
 ├── CarRental.API            → Web API layer
 ├── CarRental.Core           → Interfaces (ports)
 ├── CarRental.Domain         → Domain model (entities, exceptions)
 ├── CarRental.Infrastructure → EF Core, Identity, Email, Logging
 ├── CarRental.UseCases       → CQRS Handlers & DTOs
/tests
 ├── CarRental.Tests.Functional   → End-to-end HTTP tests
 ├── CarRental.Tests.Integration  → Database & repository integration tests
 └── CarRental.Tests.UseCases     → Unit tests for use cases
```

---

## 🧪 Testing Strategy

| Type          | Scope                          | Tools                         | Coverage Example           |
| ------------- | ------------------------------ | ----------------------------- | -------------------------- |
| ✅ Unit Tests  | Pure logic, handlers, services | `xUnit`, `NSubstitute`        | CreateRentalCommandHandler |
| ✅ Integration | Real DB queries, repos         | EF Core + SQLite/PostgreSQL   | EfRentalRepository         |
| ✅ End-to-End  | HTTP API requests              | `TestServer`, `WebAppFactory` | Rental full flow           |

To run tests:

```bash
cd tests/CarRental.Tests.UseCases
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🔐 Authentication and Authorization

- User registration and login using **JWT tokens**.
- Secure, token-based authentication with robust configuration.
- **ASP.NET Core Identity** for user, role (`Admin`, `User`), and claims management with seeding via a `Seeder`.
- Role-based route protection using `[Authorize(Roles = "Admin")]`.
- Public routes allowed with `[AllowAnonymous]`.

---

## 📦 Database Providers

- **EF Core (Entity Framework Core)** – Code-First ORM with support for multiple database engines:
  - **SQL Server**
  - **PostgreSQL**
  - **SQLite** (used for automated builds in GitHub Actions)

> The database provider can be configured via `appsettings.json` using the `DatabaseProvider` key.

---

## 🧪 Validation and Behavior

- **FluentValidation** – Declarative validation for DTOs and use-case commands.
- **Validation Pipeline** with MediatR – Automatically applies validators before command handlers.

---

## 🧰 Services and Utilities

- **AutoMapper** – Maps between domain entities and DTOs.
- **Serilog** – Structured logging, configurable via `appsettings.json`, with console and file output.
- **Swagger + Scalar** – Interactive API documentation:
  - `/swagger`, `/openapi/v1.json`, `/scalar`

---

## 📧 Email Services

> Inspired by the [`Clean.Architecture.Infrastructure`](https://github.com/ardalis/CleanArchitecture) project by Ardalis.

- Abstraction via `IEmailService` with interchangeable implementations:
  - `FakeEmailSender` for testing
  - `MimeKitEmailSender` for real email delivery

---

## 📈 Monitoring & Observability

- ✅ Health endpoints: `/health`, `/alive`
- ✅ OpenTelemetry: traces for ASP.NET and HttpClient
- ✅ Centralized logs with Serilog (console + file)

---

## 📦 EF Core Migrations

Create migrations by provider:

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



## Formalización nomenclatura métodos en repositorios (DDD + Clean Architecture)
## Método		Uso / Significado											Retorno típico			Comentario
GetByIdAsync	Obtener un único agregado o entidad por su ID				Entidad o null			Busca exacto, obligatorio único
FindByAsync		Buscar entidad por alguna(s) propiedad(es) específicas		Entidad o null			Puede no existir, búsqueda con filtro(s)
ListAllAsync	Listar todas las entidades del tipo							Lista de entidades		Sin filtro, paginación opcional
ListByAsync		Listar entidades filtradas por propiedades					Lista de entidades		Retorna 0 o más entidades, filtro aplicado
ExistsByAsync	Verificar si existe entidad con propiedades dadas			bool					Ideal para validaciones antes de creación
AddAsync		Insertar nueva entidad										void / Task				Persistencia
UpdateAsync		Actualizar entidad											void / Task				Puede incluir control de versión
DeleteAsync		Eliminar entidad (lógico o físico según implementación)		void / Task	


## Conventional Commits 
feat(car):		allow creation of car with version control
fix(car):		increment version properly on soft delete
refactor(car):	extract status logic into enum
test(car):		add unit tests for CreateCarCommandHandler

## Test Coverage & Report Generator
setup:
			dotnet tool install --global dotnet-reportgenerator-globaltool

PowerShell:
			dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults; reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
cmd.exe:    
			dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults && reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

Reporte:
			reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

Genera el archivo:
			
			./coveragereport/index.html

## Hithub Actions CI / CD

	.github/workflows/ci.yml


---

**Author / Autor**: Martín Duhalde + ChatGPT (2025)



**Author / Autor**: Martín Duhalde + ChatGPT (2025)

