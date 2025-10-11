# WO-ARCH-003 – Plan de renombrado de ensamblados Core/UseCases

## Contexto
- Situación actual: `CarRental.Core` actúa como capa de contratos/puertos, mientras que `CarRental.UseCases` implementa handlers MediatR. La nomenclatura genera confusión sobre responsabilidades.
- Objetivo: definir un plan para renombrar los ensamblados y namespaces de forma consistente (`CarRental.Application.Abstractions`, `CarRental.Application`, etc.) sin romper dependencias ni configuraciones.

## Inventario preliminar (2025-03-05)
- Proyectos afectados: `src/CarRental.Core`, `src/CarRental.UseCases`, `tests/CarRental.Tests.UseCases`, `tests/CarRental.Tests.Integration`, `tests/CarRental.Tests.Functional`.
- Referencias cruzadas:
  - `CarRental.API` depende de `CarRental.UseCases` y `CarRental.Core`.
  - `CarRental.Infrastructure` depende de `CarRental.Core` para implementar puertos.
- No se detectan atributos `InternalsVisibleTo`.
- Documentación involucrada: `docs/guides/2025-03-04-architecture-structure-notes.md`, `docs/ai-directives/work-orders.md`.

## Tareas propuestas
1. **Mapa de namespaces**
   - Generar listado de namespaces en `Core` y `UseCases` (`rg "^namespace CarRental"`).
   - Identificar aquellos consumidos externamente (API/Infrastructure/Tests).
2. **Definición de nuevo esquema**
   - Validar propuesta `CarRental.Application.Abstractions` (ports/mensajes) + `CarRental.Application` (handlers / behaviors).
   - Determinar impacto en nombres de carpetas y espacios de nombres en pruebas.
3. **Script/borrador de migración**
   - Preparar script de renombre (`dotnet sln`, `git mv`, `sed`/`PowerShell`) o guía paso a paso para refactor asistido por IDE.
   - Considerar orden: renombrar proyecto (csproj/dir) → actualizar referencias → actualizar namespaces → ejecutar `dotnet build`.
4. **Actualización de documentación**
   - Ajustar `docs/guides/2025-03-04-architecture-structure-notes.md` y `docs/guides/codex-agent-directives.md`.
5. **Validación**
   - Planificar ejecución de `dotnet build` y suites rápidas (`tests/CarRental.Tests.UseCases`) tras el renombrado.

## Pendientes antes de iniciar
- Confirmar con stakeholders el nombre final de cada capa.
- Revisar si existen paquetes NuGet generados que dependan de los nombres actuales.
