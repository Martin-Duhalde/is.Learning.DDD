# Notas de Arquitectura – Estructura de Proyectos

## Observaciones Clave
- **✅SDK actualizado (2025-03-05)**: Los proyectos `CarRental.Domain`, `CarRental.Application.Abstractions`, `CarRental.Application` y `CarRental.Infrastructure` se ejecutan con `Microsoft.NET.Sdk` y fueron validados con `dotnet build CarRental.sln`.
- **✅Dependencia corregida (2025-03-05)**: La capa de aplicación consume puertos expuestos desde `CarRental.Application.Abstractions`; no existen referencias cruzadas con Infrastructure.
- **✅Naming consolidado (2025-03-05)**: `CarRental.Core` → `CarRental.Application.Abstractions` y `CarRental.UseCases` → `CarRental.Application`, manteniendo responsabilidad clara entre contratos y orquestación.
- **✅Documentación sincronizada (2025-03-05)**: Readmes, `AGENTS.md` y directrices de agentes actualizadas con la nomenclatura vigente.

## Acciones Recomendadas
1. ✅ (2025-03-05, [WO-ARCH-001](../work-orders/06-done/WO-ARCH-001.md)): SDK de librerías en `Microsoft.NET.Sdk` y builds verificados.
2. ✅ (2025-03-05, [WO-ARCH-002](../work-orders/06-done/WO-ARCH-002.md)): Referencia de `CarRental.UseCases` a Infrastructure eliminada y sustitución por puertos en `Core`.
3. ✅ (2025-03-05, [WO-ARCH-003](../work-orders/06-done/WO-ARCH-003.md)): Renombrar `CarRental.Core` y `CarRental.UseCases` a `CarRental.Application.Abstractions` y `CarRental.Application`, incluyendo namespaces/tests/documentación.
4. Documentar estándares de naming/DI en `docs/guides/codex-agent-directives.md` tras el renombrado.

## Historial
- 2025-03-05: Actualización de SDKs y separación UseCases/Infrastructure marcada como completada.
- 2025-03-05: Renombrado Core/UseCases → Application.Abstractions/Application y actualización de pruebas/documentación.

## Órdenes de Trabajo (Work Orders)
- **WO-ARCH-001**: Actualizar SDK de proyectos de librería. Incluye ajustes de `PackageReference` y verificación de build.
- **WO-ARCH-002**: Analizar y remover dependencias Infrastructure → Application.
- **WO-ARCH-003**: Ejecutar renombrado de ensamblados (Application.Abstractions/Application) y mantener documentación alineada.

Cada orden debe registrarse con fecha de ejecución y enlaces a PR/commit.

Ver tablero Kanban: docs/work-orders/board.md
