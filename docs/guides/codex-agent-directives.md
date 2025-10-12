# Codex Agent Directives – CarRental

## Propósito
Establecer pautas para cualquier agente Codex que colabore en `DDD.CarRental`, de modo que reutilice el conocimiento recopilado y siga una estrategia coherente sin reanalizar innecesariamente el código base.

## Documentación esencial
1. **Auditoría detallada** – `docs/audits/2025-03-04-car-rental-module-audit.md`
   - Hallazgos priorizados (alta/ media/baja severidad).
   - Recomendaciones inmediatas y diseño propuesto para módulo de órdenes/pagos.
2. **Cache de sesión** – `docs/audits/2025-03-04-car-rental-session-cache.md`
   - Resumen de arquitectura actual, flujos clave, pain points y checklist de acciones.

⚠️ Antes de iniciar cualquier análisis o cambio, leer ambos archivos. Actualizar o extenderlos al terminar la sesión si surge información nueva.

## Prioridades operativas
1. Mantener consistencia con la hoja de ruta publicada (ver `docs/guides/2025-03-04-car-rental-roadmap.md`).
2. Registrar resultados relevantes en los archivos anteriores o anexar nuevos documentos bajo `docs/audits/` o `docs/guides/` con nomenclatura `YYYY-MM-DD-descripcion.md`.
3. Respetar arquitectura DDD actual: separar Dominio/Application.Abstractions/Application/Infra/API; no introducir dependencias cruzadas que violen capas.
4. Verificar que documentación, scripts y namespaces usen la nomenclatura vigente (`CarRental.Application.Abstractions`, `CarRental.Application`) antes de cerrar una tarea.
5. Para compilaciones y pruebas, solicitar al usuario la ejecución: el sandbox actual no permite completarlas con éxito y agotan recursos sin resultados útiles.

## Workflow sugerido para nuevas tareas
1. **Revisar estado actual**: leer roadmap y checklist para conocer pendientes.
2. **Planificar**: usar herramienta de plan (mínimo 2 pasos) y alinear con roadmap.
3. **Ejecutar**: preferir pruebas automatizadas existentes (`tests/CarRental.Tests.*`). Si se agregan, registrarlas.
4. **Documentar**: actualizar auditorías o crear notas bajo `docs/audits/` con hallazgos/decisiones.
5. **Comunicar**: al finalizar, recordar próximas acciones sugeridas en congruencia con roadmap.

## Convenciones
- Directorio de documentación: `docs/` subdividido en `audits/`, `guides/`, etc.
- Idioma: español técnico salvo que se requiera inglés (ej. comentarios públicos).
- Formato Markdown, títulos H1/H2, listas para tareas.
- Control de versiones optativo, pero no revertir cambios ajenos.

## Tareas pendientes clave
Ver hoja de ruta. Si no hay tareas en progreso, avanzar en la siguiente prioridad del roadmap. Si surge algo urgente, documentarlo y notificar.
