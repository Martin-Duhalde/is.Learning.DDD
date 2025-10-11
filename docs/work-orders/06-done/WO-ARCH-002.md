# WO-ARCH-002 – Desacoplar UseCases de Infrastructure

## Contexto
- Hoja de ruta: Fase 1, fortalecimiento DDD.
- Problema original: `CarRental.Application` (antes `CarRental.UseCases`) referenciaba `CarRental.Infrastructure` para acceder a `CarRentalDbContext` y `ApplicationUser`.
- Objetivo: mantener dependencias dirigidas Domain/Core → UseCases → API, evitando acoplamientos a infraestructura.

## Tareas
1. **Eliminar referencia en el proyecto** _(Completado 2025-03-05)_
   - Editar `src/CarRental.Application/CarRental.Application.csproj` (antes `CarRental.UseCases`) y remover el `<ProjectReference>` a Infrastructure.
2. **Refactorizar handler GetCarById** _(Completado 2025-03-05)_
   - Reemplazar `CarRentalDbContext` por `ICarRepository`.
   - Ajustar consultas para usar repositorio (`GetActiveByIdAsync`).
3. **Refactorizar handler de email** _(Completado 2025-03-05)_
   - Introducir puerto en `CarRental.Application.Abstractions` (ej. `IUserDirectory`) para obtener email por `UserId`.
   - Actualizar `SendReservationConfirmationEmailCommandHandler` para usar el puerto.
4. **Implementación en Infrastructure** _(Completado 2025-03-05)_
   - Crear adaptador `UserDirectory` que envuelva `UserManager<ApplicationUser>`.
   - Registrar la implementación en `DependencyInjection.AddInfrastructure`.
5. **Actualizar pruebas** _(Completado 2025-03-05)_
   - Modificar mocks de los handlers en `tests/CarRental.Tests.Application/...` para usar el nuevo puerto.
   - Ajustar suites de integración para escenarios consistentes.
6. **Revisar DI y compilar** _(Completado 2025-03-05)_
   - Verificar build y suites relevantes.

## Notas
- Añadir el puerto bajo `src/CarRental.Application.Abstractions/Interfaces/`.
- Considerar documentar el cambio en `docs/guides/2025-03-04-architecture-structure-notes.md` una vez finalizado.
- Actualizar el tablero (`docs/work-orders/board.md`) cuando cambie de estado.
