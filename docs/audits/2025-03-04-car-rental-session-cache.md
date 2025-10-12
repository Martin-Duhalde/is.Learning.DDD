# Session Knowledge Cache – CarRental

## Current Architecture Snapshot
- **Projects**: Domain, Core, UseCases, Infrastructure, API, Tests.
- **Patterns**: DDD-inspired layering + MediatR for commands/queries, FluentValidation pipeline, EF Core repositories, AutoMapper for DTO mapping.
- **Key Entities**: `Car`, `Customer`, `Rental`, `Service` (all soft-delete + version control, though invariants are externalized).
- **Critical Services**:
  - `CarRentalDbContext` with Identity integration, manual relationships, no global query filters.
  - `EfRepository<T>` base implementing soft delete, version increment, concurrency checks (partial).
  - Repos for cars, rentals, services with custom methods (`IsAvailableAsync`, `ListActivesBetweenDatesAsync`, etc.).
  - `AuthService` orchestrates Identity + Customer creation, lacks transaction scope.
  - Notification handler `SendReservationConfirmationEmailCommandHandler` uses `UserManager` + email abstraction.

## Notable Flows
- **Rental Creation**: DTO → AutoMapper → `CreateRentalCommandHandler` → availability check via `ICarRepository` → persist rental → trigger confirmation email command.
- **Rental Modification**: Validates availability for new slot and updates car/dates.
- **Cancellation**: Directly flips status via repository without concurrency guards.
- **Statistics**: Multiple queries on `IRentalRepository` for dashboards/top cars/daily usage; some rely on in-memory grouping.
- **Services Module**: `CreateServiceCommand` prevents duplicates; `GetUpcomingServicesQuery` returns schedules.
- **API Composition**: Controllers rely on MediatR + AutoMapper; Program registers MediatR thrice; fake email flag hardcoded.

## Pain Points (Condensed)
- Application layer leaking infrastructure (`GetCarByIdQueryHandler` uses DbContext).
- Reliability issues: missing transactions, concurrency checks, global filters.
- Performance risk: `CheckAvailabilityQueryHandler` N+1 pattern.
- Domain model anemic; business rules dispersed across handlers.
- DI duplication (`AddMediatR`) and inconsistent naming (`IEmailService`).

## Proposed Extensions (Reminder)
- Introduce `RentalOrder`/`Payment` aggregates, `Money` value object, domain events for rental lifecycle.
- Port interfaces: `IRentalOrderRepository`, `IPaymentRepository`, `IPaymentGateway`, `IPricingService`.
- New commands/queries for orders & payments; integrate via MediatR, AutoMapper, FluentValidation.
- Add EF mappings, repositories, fake gateway adapter, transactional boundaries.
- Extend API with `OrderController`, `PaymentController`, plus tests (unit/integration/functional).

## Next Actions Checklist
- [ ] Normalize MediatR registration & DI configuration.
- [x] (2025-03-07) Global `IsActive` query filters configurados en `CarRentalDbContext`.
- [x] (2025-03-07) Concurrency guard agregado en `EfRepository.DeleteAsync`.
- [ ] Refactor domain entities to encapsulate invariants.
- [ ] Implement order/payment module per proposal.
- [ ] Update docs/tests/migrations accordingly.
- [x] (2025-03-07, WO-ARCH-004) MediatR registration consolidated en `Program.cs`; build/tests pendientes de ejecutar fuera del sandbox.
