# CarRental Audit & Payment Module Proposal

## Context
- Repository: `DDD.CarRental`
- Scope: Review of existing rental/service modules under DDD, MediatR, EF infrastructure and proposal for orders/payments integration.
- Date: 2025-03-04

## Findings
### High Severity
1. `src/CarRental.Application/Cars/GetById/GetCarByIdQueryHandler.cs` acopla la capa de aplicación con `CarRentalDbContext` y expone entidades de dominio directamente; viola las dependencias DDD y dificulta mocked testing o migración a otros stores.
2. `src/CarRental.Application/Rentals/CheckAvailability/CheckAvailabilityQueryHandler.cs` realiza filtrado en memoria y bucles `IsAvailableAsync` (N+1). Impacto crítico en sets grandes; debería delegar en consultas agregadas repositorio o especificaciones.
3. `src/CarRental.Infrastructure/Repositories/EfRepository.cs` (`DeleteAsync`) ignora control de versiones. Incrementa versión sin comparar contra la persistida, permitiendo sobrescritura de eliminaciones concurrentes.
4. `src/CarRental.Infrastructure/Auth/AuthService.cs` registra usuario y crea `Customer` sin transacción. Falla al persistir el cliente deja usuario Identity sin cliente asociado.

### Medium Severity
1. Entidades de dominio (`src/CarRental.Domain/Entities/*.cs`) anémicas; setters públicos y sin invariantes. Reglas de negocio residiendo en handlers erosiona el modelo.
2. Registro duplicado de MediatR (`src/CarRental.API/Program.cs` en líneas ~123, 130, 168). Multiplica handlers/behaviors y complica diagnósticos.
3. `CancelAsync` en `src/CarRental.Infrastructure/Repositories/EfRentalRepository.cs` no valida estado ni versión al cancelar; riesgo de inconsistencias.
4. Falta filtro global para `IsActive` en EF (`src/CarRental.Infrastructure/Databases/CarRentalDbContext.cs`); consultas específicas pueden exponer datos lógicamente eliminados.

### Low Severity
1. Flag `fakeEmail` hardcodeado en `src/CarRental.API/Program.cs`; sin configuración para alternar proveedores.
2. Archivo `src/CarRental.Application.Abstractions/Interfaces/IEmailSender.cs` declara `IEmailService`; naming inconsistente y confuso.

## Recomendaciones
### Correcciones inmediatas
- Introducir DTOs/handlers que usen repositorios, no `DbContext`, y mapear entidades antes de exponerlas.
- Optimizar disponibilidad mediante consultas agregadas (p. ej. repositorio con `IsAvailableInRangeAsync` que use una sola query).
- Revisar `EfRepository` para validar versiones antes de borrar y usar actualizaciones con `ConcurrencyStamp` o tokens.
- Encapsular registro de usuario + cliente en transacción (`IDbContextTransaction` o `IExecutionStrategy`).
- Añadir filtros globales `HasQueryFilter(e => e.IsActive)`.
- Consolidar una única llamada `AddMediatR` registrando todos los ensamblados necesarios.

## Módulo Órdenes/Pagos
### Dominio
- Agregado `RentalOrder` (Pending, Paid, Cancelled, Refunded) con entidad hija `Payment` y VOs `Money`, `PaymentMethod`.
- Métodos ricos (`MarkAsPaid`, `Cancel`, `RegisterPayment`) que disparen eventos (`OrderPaidDomainEvent`, `OrderCancelledDomainEvent`).
- `Rental` emite `RentalCreatedDomainEvent` y `RentalCancelledDomainEvent` para sincronizar órdenes.

### Application Abstractions
- `IRentalOrderRepository`, `IPaymentRepository` derivados de `IRepository<T>`.
- `IPricingService` (calcula fees, impuestos, depósitos) y `IPaymentGateway` (adapta Stripe, MercadoPago, etc.).

### Casos de uso (MediatR)
- Comandos: `CreateRentalOrderCommand`, `ProcessPaymentCommand`, `RefundPaymentCommand`, `CancelOrderCommand`.
- Queries: `GetOrderDetailsQuery`, `ListCustomerOrdersQuery`.
- Validación via FluentValidation y `ValidationBehavior` existente.

### Infraestructura
- Ampliar `CarRentalDbContext` con `DbSet<RentalOrder>` y `DbSet<Payment>`; mapear `Money` vía owned types.
- Repos `EfRentalOrderRepository`, `EfPaymentRepository` con control de concurrencia.
- Adaptadores de gateway: `FakePaymentGateway` (tests) y espacio para real.
- Gestionar transacciones alrededor de `CreateRentalCommandHandler` y creación de órdenes mediante `Unit of Work`.

### API y Presentación
- `OrderController`: `POST /api/orders/{rentalId}`, `GET /api/orders/{id}`.
- `PaymentController`: `POST /api/orders/{id}/pay`, `POST /api/orders/{id}/refund`.
- DTOs con AutoMapper (`OrderDto`, `PaymentDto`, `CreateOrderResponseDto`).

### Eventos & Procesos
- Registrar ensamblado nuevo una sola vez con `builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateRentalOrderCommand).Assembly));`.
- Handlers de notificación: `OrderPaidNotificationHandler` para disparar correos/webhooks.
- Opcional: integrar colas (RabbitMQ/Azure Service Bus) si se requiere resiliencia al notificar pagos.

### Testing
- Unit tests de invariantes (`RentalOrderTests`, `PaymentTests`).
- Integration tests en `tests/CarRental.Tests.Integration` cubriendo flujos: creación de orden, pago exitoso, rechazo, reembolso.
- Functional API tests en `tests/CarRental.Tests.Functional` para endpoints orden/pago.

## Próximos Pasos Sugeridos
1. Normalizar registro MediatR/DI y añadir filtros globales + controles de concurrencia.
2. Refactorizar entidades para encapsular lógica de negocio antes de expandir el dominio.
3. Implementar módulo descrito con migraciones y pruebas automáticas.
