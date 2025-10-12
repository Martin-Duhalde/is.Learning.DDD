# WO-ARCH-005 – Normalizar filtros globales `IsActive`

## Contexto
- Hallazgo crítico en `docs/audits/2025-03-04-car-rental-module-audit.md`: el `DbContext` no aplicaba filtros globales, exponiendo entidades soft-deleted.
- Se observaron múltiples consultas y repositorios replicando `Where(e => e.IsActive)` para compensar la ausencia del filtro.

## Objetivo
Aplicar `HasQueryFilter` para todas las entidades con `IsActive` y ajustar repositorios/pruebas para operar correctamente con el nuevo comportamiento.

## Acciones realizadas
1. Agregados filtros globales para `Customer`, `Car`, `Rental` y `Service` en `src/CarRental.Infrastructure/Databases/CarRentalDbContext.cs`.
2. Limpiadas condiciones redundantes `IsActive` en `EfCarRepository` y `EfServiceRepository`, documentando el uso de los filtros.
3. Incorporado `IgnoreQueryFilters` en `EfRepository.DeleteAsync` y pruebas de integración para validar soft delete y excepciones de negocio.
4. Actualizados tests de integración (`CarRentalDbContextTests`, `EfRepositoryTest`) con `IgnoreQueryFilters` y nuevas aserciones, más comentarios explicativos para desarrolladores junior.

## Criterios de aceptación cumplidos
- El `DbContext` oculta por defecto entidades con `IsActive = false`.
- Los repositorios funcionan sin duplicar lógica de filtrado y levantan excepciones correctas tras soft-delete.
- Pruebas de integración relevantes (`Should_Persist_IsActive_LogicalDelete`, `should_soft_delete_entity_and_increment_version`, `should_throw_if_entity_already_deleted_when_deleting`) ajustadas y en verde.
- Documentación sincronizada (`docs/audits/2025-03-04-car-rental-session-cache.md`, `docs/guides/codex-agent-directives.md`).

## Validación
- `dotnet test tests/CarRental.Tests.Integration/CarRental.Tests.Integration.csproj` ejecutado por el usuario con resultado satisfactorio.

## Seguimiento
- Responsable: Codex (GPT-5).
- Dependencias: ninguna adicional; cambios listos para commit por el usuario.
