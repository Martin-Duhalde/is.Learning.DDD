# WO-ARCH-008 – Encapsular invariantes del dominio (Fase 2)

## Contexto
- Roadmap Fase 2 exige fortalecer el modelo rico: `Car`, `Rental`, `Service` continúan con setters públicos y reglas repartidas en handlers.
- Auditoría señala riesgo medio por entidades anémicas.

## Objetivo
Mover reglas y validaciones clave dentro de los agregados, introduciendo métodos y constructores que garanticen invariantes sin depender de código externo.

## Alcance inicial
- `Car`: asegurar creación con estado consistente y métodos para cambios controlados.
- `Rental`: encapsular calendario (start/end), asignación de auto y estados (Active/Cancelled) con control de versiones.
- `Service`: revisar programación de mantenimientos y validaciones de fechas.

## Tareas
- [x] Diseñar constructores/métodos factory para `Car` evitando setters públicos.
- [x] Ajustar `ICarRepository`/`EfCarRepository` y tests que dependían del patrón anémico.
- [ ] Extender el enfoque a `Rental` y `Service` (factories + repos específicos).
- [ ] Migrar handlers restantes (`CreateRental`, `ModifyRental`, etc.) al modelo rico.
- [ ] Actualizar documentación (guías/auditoría) con nuevas invariantes.

## Dependencias
- Ninguna bloqueante; se apoya en infraestructura ya consolidada (filtros `IsActive`, cancelaciones).

## Evidencia pendiente
- Ejecutar `dotnet test tests/CarRental.Tests.Application/CarRental.Tests.Application.csproj`
- Ejecutar `dotnet test tests/CarRental.Tests.Integration/CarRental.Tests.Integration.csproj`
