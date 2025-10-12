# WO-ARCH-004 – Consolidar registro MediatR

## Contexto
- Hallazgo severidad media en `docs/audits/2025-03-04-car-rental-module-audit.md`: múltiples llamadas `AddMediatR` en `src/CarRental.API/Program.cs` duplican handlers y behaviors.
- Consolidar el registro prepara la base para futuras extensiones (órdenes/pagos) sin efectos colaterales en MediatR.

## Objetivo
Unificar la configuración de MediatR en la API en una única llamada que registre handlers relevantes y el `ValidationBehavior`.

## Plan de acción
1. Revisar las secciones actuales de `builder.Services.AddMediatR` en `src/CarRental.API/Program.cs` para enumerar assemblies y behaviors registrados. ✅
2. Diseñar la configuración consolidada asegurando que incluye `Assembly.GetExecutingAssembly()`, ensamblados de aplicación y `ValidationBehavior<,>` en el pipeline. ✅
3. Reemplazar las tres llamadas existentes por la nueva configuración unificada y limpiar comentarios obsoletos. ✅
4. Ejecutar `dotnet build CarRental.sln` (y pruebas rápidas si aplica) para validar que los handlers continúan resolviéndose correctamente. ✅ (ejecutado por usuario, build + tests OK)
5. Actualizar la documentación correspondiente (`docs/audits/2025-03-04-car-rental-session-cache.md`) con la decisión tomada. ✅

## Criterios de aceptación
- Solo existe una llamada a `AddMediatR` en `Program.cs` y registra `ValidationBehavior<,>`.
- Los handlers y validadores previos siguen disponibles sin registros duplicados.
- Build (`dotnet build CarRental.sln`) finaliza correctamente tras el cambio.
- Se documenta la consolidación en la sesión o guía correspondiente.

## Seguimiento
- Responsable actual: Codex (GPT-5).
- Dependencias: ninguna externa; requiere permisos de escritura en `src/CarRental.API/Program.cs`.
- Estado: criterios de aceptación cumplidos, listo para mover a “Done”.
