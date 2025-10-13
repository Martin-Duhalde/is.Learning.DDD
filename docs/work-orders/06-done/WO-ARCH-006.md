---
wo: WO-ARCH-006
title: Normalizar contratos HTTP del CarController
status: Done
completed: 2025-03-08
---

## Contexto
- Se detectó que `CarController` exponía comandos de aplicación como contratos HTTP, lo que acoplaba la API a MediatR.
- Además, AutoMapper sólo se registraba para el perfil de rentals, dejando fuera perfiles de autos y servicios.

## Acciones
- Se inyectó `IMapper` en `CarController` para mapear `CreateCarRequestDto` y `UpdateCarDto` hacia los comandos antes de invocarlos con MediatR (`src/CarRental.API/Controllers/CarController.cs`).
- Se ajustó el bootstrap de AutoMapper para registrar toda la asamblea de `CarRental.Application` (`src/CarRental.API/Program.cs`).
- Se documentaron los cambios y la necesidad de validar pruebas fuera del sandbox.

## Evidencia
- Tests (ejecutados por el usuario): `dotnet test tests/CarRental.Tests.Functional/CarRental.Tests.Functional.csproj`

## Decisiones
- Mantener los DTOs de aplicación como contratos HTTP —no se duplican en la API— pero se evita exponer los comandos directamente gracias al mapeo con AutoMapper.
- La configuración de AutoMapper pasa a discovery por ensamblado para simplificar futuros perfiles.

## Seguimiento
- No se identificaron tareas adicionales; el tablero queda limpio tras cerrar la orden.
