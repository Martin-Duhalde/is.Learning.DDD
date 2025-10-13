# WO-ARCH-007 – Optimizar disponibilidad de autos sin N+1

## Contexto
- Hallazgo de severidad alta en la auditoría 2025-03-04: `CheckAvailabilityQueryHandler` hacía `ListAllActivesAsync` + `IsAvailableAsync` por cada auto, generando N+1 queries.
- Con herramientas actuales (EF Core + repositorios) se podía producir un único round-trip que cruce autos activos y rentals activos.

## Acciones
- Se creó `ICarAvailabilityReadService` y su implementación EF `CarAvailabilityReadService`, registrada en DI (`src/CarRental.Infrastructure/Services/CarAvailabilityReadService.cs`).
- `CheckAvailabilityQueryHandler` usa ahora la nueva abstracción para obtener la lista filtrada en un solo paso (`src/CarRental.Application/Rentals/CheckAvailability/CheckAvailabilityQueryHandler.cs`).
- Pruebas unitarias/integración fueron actualizadas para validar la interacción y el resultado (`tests/CarRental.Tests.Application/Rentals/CheckAvailabilityQueryHandlerTests.cs`, `tests/CarRental.Tests.Integration/Rentals/CheckAvailabilityQueryHandlerTests.cs`).
- Documentación del tablero actualizada para reflejar que la orden está cerrada.

## Justificación de la abstracción
- `ICarRepository` permanece centrado en operaciones sobre el agregado (CRUD, consultas por Id). La consulta de disponibilidad cruza información de otra agregación (`Rental`), por lo que se encapsuló en un servicio de lectura dedicado, facilitando futuras optimizaciones/caché sin acoplar Application a EF.

## Evidencia
- Tests ejecutados por el usuario:
  - `dotnet test tests/CarRental.Tests.Application/CarRental.Tests.Application.csproj`
  - `dotnet test tests/CarRental.Tests.Integration/CarRental.Tests.Integration.csproj`

## Resultado
- El endpoint de disponibilidad evita el patrón N+1 y se mantiene alineado con la arquitectura por capas.
