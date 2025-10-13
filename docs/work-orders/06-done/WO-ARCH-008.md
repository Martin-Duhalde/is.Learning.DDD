# WO-ARCH-008 – Encapsular invariantes del dominio (Fase 2)

## Contexto
- Roadmap Fase 2 exige fortalecer el modelo rico: `Car`, `Rental`, `Service` continúan con setters públicos y reglas repartidas en handlers.
- Auditoría señalaba riesgo medio por entidades anémicas.

## Acciones
- `Car` migrado a agregado rico con fábricas `Create/Restore/ForTesting`, getters privados e invariantes encapsuladas (`src/CarRental.Domain/Entities/Car.Domain.cs`).
- `ICarRepository`/`EfCarRepository`/decorador de caché reescritos para operar con el agregado y mantener control de concurrencia (`src/CarRental.Application.Abstractions/Repositories/ICarRepository.cs`, `src/CarRental.Infrastructure/Repositories/EfCarRepository.cs`, `src/CarRental.Infrastructure/Caching/CarCachedRepository.cs`).
- Handlers y tests de aplicación, integración y funcional actualizados para usar helpers de dominio (`DomainBuilder`, `Car.ForTesting`), evitando inicializadores anémicos. Suite completa superó 210 pruebas.

## Evidencia
- Compilación `dotnet build CarRental.sln` exitosa (2025-03-08).
- Tests locales ejecutados por el usuario.

## Seguimiento
- Mantener la estrategia para `Rental` y `Service` (ver nuevas órdenes WO-ARCH-009/010 antes de retirar `IEntity`).
