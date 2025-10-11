# WO-ARCH-003 – Renombrar Application.Abstractions / Application

## Contexto
- Objetivo: alinear la capa de contratos y la de orquestación bajo los nombres `CarRental.Application.Abstractions` y `CarRental.Application`, reemplazando los antiguos `CarRental.Core` y `CarRental.UseCases`.
- Alcance: actualizar proyectos, namespaces, pruebas automatizadas y documentación operativa para evitar ambigüedades de naming en la arquitectura.

## Ejecución
1. **Estructura de proyectos** _(2025-03-05)_
   - Renombrados directorios y `*.csproj` para `CarRental.Application.Abstractions` y `CarRental.Application`, ajustando `CarRental.sln` y referencias en API, Infrastructure y suites de pruebas.
2. **Namespaces y pruebas** _(2025-03-05)_
   - Sustituido `CarRental.Core.*` por `CarRental.Application.Abstractions.*` y `CarRental.UseCases.*` por `CarRental.Application.*` en código y tests (`tests/CarRental.Tests.*`).
   - Sincronizados archivos de configuración (por ejemplo, `launchSettings.json`) con las nuevas rutas.
3. **Documentación** _(2025-03-05)_
   - Actualizados `readme.md`, `readme-en.md`, `AGENTS.md` y `docs/guides/codex-agent-directives.md` con la nomenclatura vigente y recomendaciones para limpiar restos locales.
   - Registrado el cierre en las notas de arquitectura y en el tablero Kanban.

## Evidencia de pruebas
- `dotnet build CarRental.sln`
- `dotnet test tests/CarRental.Tests.Application/CarRental.Tests.Application.csproj`
- `dotnet test tests/CarRental.Tests.Integration/CarRental.Tests.Integration.csproj`
- `dotnet test tests/CarRental.Tests.Functional/CarRental.Tests.Functional.csproj`

## Resultado
- Ensamblados y namespaces unificados bajo `CarRental.Application.Abstractions` / `CarRental.Application`.
- Documentación y directrices reflejan los nuevos nombres, reduciendo riesgo de confusión en agentes futuros.
- Tablero actualizado con la orden en estado “Done”.
