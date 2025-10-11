# Repository Guidelines

## Project Structure & Module Organization
- **Solution root:** `CarRental.sln` wires the application, infrastructure, domain, and tests.
- **Source code:** `src/CarRental.*` is layered Domain → Application.Abstractions → Application → Infrastructure → API; keep dependencies flowing en esa dirección.
- Nota histórica 2025-03-05: los ensamblados `CarRental.Core` y `CarRental.UseCases` fueron renombrados a `CarRental.Application.Abstractions` y `CarRental.Application`; elimina restos locales antes de compilar.
- **Tests:** `tests/CarRental.Tests.*` mirrors the layer split (`Application`, `Integration`, `Functional`). Add new suites beside the feature you extend.
- **Documentation:** `docs/` holds audits, guides, and work orders. Always check `docs/ai-directives/` and `docs/work-orders/board.md` before coding.

## Build, Test, and Development Commands
- `dotnet build CarRental.sln` — compile all projects and verify references.
- `dotnet test tests/CarRental.Tests.Application/CarRental.Tests.Application.csproj` — fast feedback for application logic.
- `dotnet test tests/CarRental.Tests.Integration/CarRental.Tests.Integration.csproj` — validate EF Core + infrastructure behavior.
- `dotnet test tests/CarRental.Tests.Functional/CarRental.Tests.Functional.csproj` — end-to-end API scenarios (slower).
- Use `dotnet format` only when coordinated; respect existing formatting.

## Coding Style & Naming Conventions
- C# files use **4-space indentation**, `PascalCase` for types, `camelCase` for locals/fields with `_` prefix for private readonly (e.g., `_carRepository`).
- Favor expression-bodied members for simple getters; keep constructors explicit.
- Namespace pattern: `CarRental.<Layer>.<Feature>` (e.g., `CarRental.Application.Rentals.Create`).
- Avoid direct infrastructure references en Domain/Application.Abstractions/Application; introduce ports under `CarRental.Application.Abstractions.Interfaces` o `Repositories`.

## Testing Guidelines
- Framework: xUnit with NSubstitute (Application) y Moq (Integration). Sigue el patrón `should_<behavior>_when_<condition>`.
- Cover new behaviors with the narrowest test type first; extend to integration only when persistence or DI is touched.
- Keep in-memory EF tests consistent: seed data through repositories or shared context instances.

## Commit & Pull Request Guidelines
- Commit messages follow `<type>: <summary>` lowercase type (e.g., `refactor: desacoplar casos de uso de identidad`). Group related changes per commit.
- Pull requests should describe the problem, the approach, and testing evidence. Link work orders (e.g., `WO-ARCH-002`) and attach relevant docs or screenshots.
- Update roadmap or work-order notes when decisions shift architecture or workflows.

## Agent Workflow Essentials
- Start every session by reading `agent.md`, the AI directives, and active items in `docs/work-orders/in-progress/`.
- Log intermediate findings in `docs/work-orders/in-progress/WO-ARCH-XXX-progress.md` or similar before committing.
- Preserve ASCII text unless the target file already uses UTF-8 accents. Coordinate any config changes with future agents via `docs/guides/`.
