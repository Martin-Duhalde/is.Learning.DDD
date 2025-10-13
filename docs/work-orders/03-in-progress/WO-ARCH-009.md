excelente # WO-ARCH-009 – Encapsular invariantes de Rental

## Objetivo
Migrar `Rental` a un agregado rico: fábricas, métodos de negocio (`Reschedule`, `Cancel`), setters privados y control interno de invariantes.

## Alcance
- Reescribir `Rental` (`CarRental.Domain/Entities/Rental.*`).
- Sustituir `IRentalRepository`/`EfRentalRepository` por contratos específicos.
- Actualizar comandos y tests relacionados.

## Checklist
- [ ] Dominio actualizado (`Rental.Domain.cs`).
- [ ] Repositorio específico + decoradores adaptados.
- [ ] Handlers/tests ajustados.
- [ ] Documentación y regresiones analizadas.
