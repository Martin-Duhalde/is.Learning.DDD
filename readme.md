# 🇪🇸 Sistema de Alquiler de Autos - Backend (.NET 9)

Este repositorio contiene el backend de un sistema de alquiler de autos desarrollado con **.NET 9**, siguiendo los principios de **Clean Architecture**, **DDD** y **CQRS**. Está diseñado para integrarse con un frontend en Angular y soporta funcionalidades como autenticación, reservas de vehículos y seguimiento de servicios.

> ⚠️ Este repositorio fue creado para una evaluación técnica y puede eliminarse después de la entrevista.

---

## ⚙️ Tecnologías y arquitectura

- **.NET 9**, C# 12
- **Arquitectura Limpia**, **DDD** y **CQRS**
- **Entity Framework Core** con migraciones code-first
- Autenticación con **JWT** y autorización basada en roles
- **xUnit**, **FluentAssertions**, **NSubstitute**
- **OpenTelemetry**, HealthChecks, logging centralizado
- **Swagger/OpenAPI** con XML docs y soporte JWT
- **Azure Durable Functions** (preparado para envío de emails)

---

## 🚀 Cómo iniciar

```bash
cd src/CarRental.API
dotnet ef database update
dotnet run
```

Acceder a Swagger:

```
https://localhost:7263/swagger
https://localhost:7263/swagger/index.html
```

Salud y disponibilidad:

```
https://localhost:7263/health
https://localhost:7263/alive
```

---

## 🗂️ Estructura del proyecto

```
/src
 ├── CarRental.API            → Capa API REST
 ├── CarRental.Core           → Interfaces de puertos
 ├── CarRental.Domain         → Modelo de dominio
 ├── CarRental.Infrastructure → EF Core, Identity, Email, Logging
 ├── CarRental.UseCases       → Handlers CQRS y DTOs
/tests
 ├── CarRental.Tests.Functional   → Tests de extremo a extremo
 ├── CarRental.Tests.Integration  → Tests con DB y repos
 └── CarRental.Tests.UseCases     → Tests unitarios de casos de uso
```

---

## 🧪 Estrategia de testing

| Tipo                | Cobertura                      | Herramientas                  | Ejemplo                    |
| ------------------- | ------------------------------ | ----------------------------- | -------------------------- |
| ✅ Unit Tests        | Lógica pura, servicios, reglas | `xUnit`, `NSubstitute`        | CreateRentalCommandHandler |
| ✅ Integración       | Repositorios, queries reales   | EF Core + SQLite/PostgreSQL   | EfRentalRepository         |
| ✅ Extremo a Extremo | Flujo HTTP completo            | `TestServer`, `WebAppFactory` | Rental full flow           |

Ejecutar tests:

```bash
cd tests/CarRental.Tests.UseCases
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🔐 Autenticación y autorización

- Registro/Login con JWT
- Rutas por rol (`[Authorize(Roles = "Admin")]`)
- Rutas públicas (`[AllowAnonymous]`)

---

## 📈 Observabilidad

- ✅ Endpoints de salud: `/health`, `/alive`
- ✅ Trazas vía OpenTelemetry (ASP.NET y HttpClient)
- ✅ Logs centralizados con Serilog (consola + archivo)

---

## 📦 Migraciones EF Core

Crear migraciones por proveedor:

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

**Author / Autor**: Martín Duhalde + ChatGPT (2025)

