# WO-ARCH-002 – Bitácora

## Estado final (2025-03-05)
- Orden completada tras validar build y suites en Ubuntu y Windows.
- Core, Domain, UseCases e Infrastructure ahora usan `Microsoft.NET.Sdk`, manteniendo capas como librerías puras.
- `GlobalUsings` de Infrastructure reducido a `Microsoft.Extensions.Logging` para evitar dependencias implícitas.

## Evidencia técnica
- `dotnet build CarRental.sln` (Release) sin warnings.
- `dotnet test CarRental.sln` — 212 pruebas, 0 fallos, 0 omitidas (~11s).

## Seguimiento
- Pendiente documentar refactors mayores en `docs/guides/2025-03-04-architecture-structure-notes.md` si aplica.
