# Hoja de Ruta – DDD.CarRental

## Visión
Consolidar la plataforma CarRental como backend robusto DDD con soporte para órdenes y pagos, garantizando consistencia transaccional, escalabilidad y mantenibilidad.

## Objetivos estratégicos
1. **Fortalecer fundamentos DDD**
   - Encapsular invariantes en agregados (`Car`, `Rental`, `Service`).
   - Eliminar dependencias de infraestructura en capa de aplicación.
   - Implementar filtros globales, control de concurrencia y transacciones.
2. **Expandir dominio con módulo de órdenes/pagos**
   - Diseñar agregados `RentalOrder` y `Payment` con eventos de dominio.
   - Integrar pricing, gateway de pagos y casos de uso MediatR asociados.
   - Exponer API consistente para órdenes y pagos.
3. **Calidad operativa**
   - Cubrir flujos clave con pruebas unitarias, de integración y funcionales.
   - Documentar decisiones y guías de contribución.
   - Preparar pipelines (futuros) para despliegues y métricas.

## Fases
### Fase 1 – Endurecer base existente (Prioridad Alta)
- [ ] Consolidar registro único de MediatR y limpiar DI.
- [ ] Añadir filtros `IsActive` globales en `CarRentalDbContext`.
- [ ] Revisar `EfRepository`/`EfRentalRepository` para control de versiones y transacciones (incluido flujo `AuthService`).
- [ ] Refactorizar handlers que usan `DbContext` directo (`GetCarByIdQueryHandler`).
- [ ] Optimizar `CheckAvailabilityQueryHandler` con consulta especializada.

### Fase 2 – Modelo rico e invariantes (Prioridad Media)
- [ ] Migrar entidades a patrón rich domain (propiedades privadas, métodos internos).
- [ ] Centralizar validaciones de negocio en agregados y servicios de dominio.
- [ ] Revisar pipeline de validaciones para reducir duplicación.

### Fase 3 – Órdenes y pagos (Prioridad Alta una vez Fase 1 lista)
- [ ] Diseñar esquema de datos para `RentalOrder`/`Payment` + migraciones.
- [ ] Implementar interfaces `IRentalOrderRepository`, `IPaymentRepository`, `IPaymentGateway`, `IPricingService`.
- [ ] Crear comandos/queries y controladores API.
- [ ] Integrar eventos de dominio desde `Rental` a órdenes.
- [ ] Desarrollar pruebas unitarias/integración/end-to-end para flujos de pago.

### Fase 4 – Operabilidad y extensiones (Prioridad Media/Baja)
- [ ] Configurar proveedores reales de email/pagos (habilitar toggles por configuración).
- [ ] Implementar caching decorador (`CachedRepository`) si se justifica.
- [ ] Revisar monitoreo, logging y pipelines CI/CD.

## Métricas de éxito
- Cobertura de pruebas ampliada en módulos core y nuevos.
- Tiempo de respuesta de disponibilidad < 150 ms en datasets medianos.
- Tasa de fallas concurrentes reducida (validada con pruebas). 
- Documentación actualizada tras cada fase.

## Control de avances
- Actualizar checklists al finalizar tareas.
- Registrar hitos en `docs/audits` o nuevas guías.
- Mantener comunicación en commits/PRs referenciando la fase correspondiente.

