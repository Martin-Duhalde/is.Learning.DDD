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

Acceder a Scalar:

```
https://localhost:7263/scalar
https://localhost:7263/openapi/v1.json
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
- Autenticación basada en tokens con configuración segura.
- ASP.NET Core Identity
– Manejo de usuarios, roles (`Admin`, `User`) y claims con sembrado inicial (`Seeder`).
- Rutas por rol (`[Authorize(Roles = "Admin")]`)
- Rutas públicas (`[AllowAnonymous]`)

### 📦 Base de datos
- **EF Core (Entity Framework Core)** – ORM con Code-First y soporte para múltiples proveedores:
  - SQL Server
  - PostgreSQL
  - SQLite (usado en GitHub Actions)

### 🧪 Validaciones y comportamiento
- **FluentValidation** – Validación declarativa para DTOs y comandos.
- **Pipeline de validación en MediatR** – Interceptor para aplicar validaciones automáticamente.

### 🧰 Servicios y herramientas
- **AutoMapper** – Mapeo entre entidades y DTOs.
- **Serilog** – Logging estructurado configurable desde `appsettings.json`.
- **Swagger + Scalar** – Documentación interactiva de la API:
  - `/swagger`, `/openapi/v1.json`, `/scalar`

### 📧 Servicios de correo electrónico
> Inspired by the [`Clean.Architecture.Infrastructure`](https://github.com/ardalis/CleanArchitecture) project by Ardalis.
- Abstracción con `IEmailService`, implementaciones intercambiables (`FakeEmailSender`, `MimeKitEmailSender`).

---

## 📈 Observabilidad
> Inspired by the [`Clean.Architecture.Infrastructure`](https://github.com/ardalis/CleanArchitecture) project by Ardalis.
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
[setup] dotnet tool install --global dotnet-reportgenerator-globaltool

[PowerShell] dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults; reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
[cmd.exe]    dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults && reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

Genera el archivo: ./DDD.CarRental/coveragereport/index.html

## Hithub Actions CI / CD

ci.yml

---

**Author / Autor**: Martín Duhalde + ChatGPT (2025)

