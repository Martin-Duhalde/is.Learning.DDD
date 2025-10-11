# Notas de Arquitectura – Estructura de Proyectos

## Observaciones Clave
- **SDK actualizado (2025-03-05)**: Los proyectos `CarRental.Domain`, `CarRental.Core`, `CarRental.UseCases` e `CarRental.Infrastructure` fueron migrados a `Microsoft.NET.Sdk` y validados con `dotnet build CarRental.sln`.
- **Dependencia corregida (2025-03-05)**: `CarRental.UseCases` ya no referencia a `CarRental.Infrastructure`; los puertos necesarios viven en `CarRental.Core`.
- **Naming**: "+Core" sigue siendo ambiguo; describe contratos y servicios compartidos. "UseCases" es correcto si se mantiene aislado de infraestructura.

## Acciones Recomendadas
1. ✅ (2025-03-05, [WO-ARCH-001](../work-orders/06-done/WO-ARCH-001.md)): SDK de librerías en `Microsoft.NET.Sdk` y builds verificados.
2. ✅ (2025-03-05, [WO-ARCH-002](../work-orders/06-done/WO-ARCH-002.md)): Referencia de `CarRental.UseCases` a Infrastructure eliminada y sustitución por puertos en `Core`.
3. Alinear nomenclatura:
   - Opcional: renombrar `CarRental.Core` → `CarRental.Application.Abstractions`.
   - Confirmar que `CarRental.UseCases` quede como `CarRental.Application` si evoluciona a orquestador principal.
4. Documentar estándares de naming/DI en `docs/guides/codex-agent-directives.md` cuando se complete la cleanup.

## Historial
- 2025-03-05: Actualización de SDKs y separación UseCases/Infrastructure marcada como completada.

## Órdenes de Trabajo (Work Orders)
- **WO-ARCH-001**: Actualizar SDK de proyectos de librería. Incluye ajustes de `PackageReference` y verificación de build.
- **WO-ARCH-002**: Analizar y remover dependencias Infrastructure → UseCases. Si es necesario, introducir interfaz en Core.
- **WO-ARCH-003**: Proponer plan de renombrado de ensamblados (Core/UseCases) y preparar script de migración de namespaces si se aprueba.

Cada orden debe registrarse con fecha de ejecución y enlaces a PR/commit.

Ver tablero Kanban: docs/work-orders/board.md
