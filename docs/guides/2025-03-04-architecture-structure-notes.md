# Notas de Arquitectura – Estructura de Proyectos

## Observaciones Clave
- **SDK inapropiado**: Los proyectos `CarRental.Domain`, `CarRental.Core`, `CarRental.UseCases` e `Infrastructure` usan `Microsoft.NET.Sdk.Web`, agregando dependencias no necesarias para librerías.
- **Dependencia invertida**: `CarRental.UseCases` referencia directamente a `CarRental.Infrastructure`, rompiendo el flujo Domain → Core → UseCases → API.
- **Naming**: "+Core" resulta ambiguo; describe contratos y servicios compartidos. "UseCases" es correcto si se mantiene aislado de infraestructura.

## Acciones Recomendadas
1. Cambiar SDK de los proyectos de librería a `Microsoft.NET.Sdk` y validar builds/pruebas.
2. Eliminar la referencia de `CarRental.UseCases` a `CarRental.Infrastructure`; mover cualquier dependencia real a puertos en `Core`.
3. Alinear nomenclatura:
   - Opcional: renombrar `CarRental.Core` → `CarRental.Application.Abstractions`.
   - Confirmar que `CarRental.UseCases` quede como `CarRental.Application` si evoluciona a orquestador principal.
4. Documentar estándares de naming/DI en `docs/guides/codex-agent-directives.md` cuando se complete la cleanup.

## Órdenes de Trabajo (Work Orders)
- **WO-ARCH-001**: Actualizar SDK de proyectos de librería. Incluye ajustes de `PackageReference` y verificación de build.
- **WO-ARCH-002**: Analizar y remover dependencias Infrastructure → UseCases. Si es necesario, introducir interfaz en Core.
- **WO-ARCH-003**: Proponer plan de renombrado de ensamblados (Core/UseCases) y preparar script de migración de namespaces si se aprueba.

Cada orden debe registrarse con fecha de ejecución y enlaces a PR/commit.

Ver tablero Kanban: docs/work-orders/board.md
