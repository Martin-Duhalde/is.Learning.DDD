# WO-ARCH-001 – Actualizar SDK de proyectos de librería

## Contexto
- Objetivo: migrar proyectos de capas Domain/Core/UseCases/Infrastructure desde `Microsoft.NET.Sdk.Web` a `Microsoft.NET.Sdk` para eliminar dependencias innecesarias y reforzar la separación de capas.
- Referencias: `docs/guides/2025-03-04-architecture-structure-notes.md`, `docs/audits/2025-03-04-car-rental-module-audit.md`.

## Ejecución
1. **Migración de SDKs** _(2025-03-05)_
   - Editados `src/CarRental.Domain/CarRental.Domain.csproj`, `src/CarRental.Core/CarRental.Core.csproj`, `src/CarRental.UseCases/CarRental.UseCases.csproj` y `src/CarRental.Infrastructure/CarRental.Infrastructure.csproj` para usar `Microsoft.NET.Sdk`.
   - Ajustes menores a propiedades (mantener `OutputType=Library`, documentación XML y warnings suprimidos cuando aplicaba).
2. **Validación de compilación**
   - `dotnet build CarRental.sln` ejecutado en entorno local (Release/Debug) sin errores ni advertencias nuevas.
3. **Seguimiento**
   - Actualizadas notas de arquitectura con el estado final de la migración.

## Evidencia de pruebas
- `dotnet build CarRental.sln`
- `dotnet test CarRental.sln`

## Resultado
- Capas de librería libres de dependencias Web.
- Base consistente para siguientes refactors de DI/naming (WO-ARCH-003).
