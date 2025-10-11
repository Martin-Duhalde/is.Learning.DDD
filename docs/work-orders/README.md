# Sistema de Órdenes de Trabajo

## Propósito
Centralizar las iniciativas técnicas en un tablero estilo Kanban para que cualquier desarrollador o agente (humano o IA) pueda conocer el estado de cada orden, avanzar tareas y registrar traspasos a testing o despliegue.

## Flujo Kanban
1. **Backlog** – Ideas aprobadas pero aún sin priorización.
2. **Ready** – Lista priorizada, con criterios de aceptación claros.
3. **In Progress** – Trabajo activo.
4. **Code Review** – Cambios listos para revisión.
5. **Testing** – En validación (unitaria, integración o QA manual).
6. **Done** – Aprobado y mergeado/desplegado.
7. **Blocked** – Impedimentos, dependencia externa o espera de decisión.

Mover cada orden entre columnas actualizando el archivo `board.md` con la fecha y responsable.

## Naming de Órdenes
Formato `WO-<ÁREA>-<NÚMERO>` (ej. `WO-ARCH-001`). Todas las órdenes deben vincularse a documentación de contexto (`docs/audits` o `docs/guides`).

## Checklist al abrir una orden
- Descripción concisa.
- Antecedente/documento.
- Criterios de aceptación.
- Riesgos conocidos.
- Estimación (si aplica).

## Checklist al cerrar
- Validación de criterios de aceptación.
- Pruebas ejecutadas y resultados.
- Documentación o notas actualizadas.
- Referencia a PR/commit.

## Automatización futura
- Posibilidad de exportar a YAML/JSON para consumo por otras herramientas.
- Integración con scripts que generen subtareas o plantillas de PR.
