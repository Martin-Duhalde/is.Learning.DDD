## 2025-03-05 – Avance temporal

- Se reemplazó `CarRentalDbContext` por `ICarRepository` en `src/CarRental.UseCases/Cars/GetById/GetCarByIdQueryHandler.cs`, siguiendo el patrón de `ListAllCarsQueryHandler`.
- Handler de confirmación de reserva desacoplado de Identity mediante puerto `IUserDirectory`; implementación `UserDirectory` registrada en Infrastructure y pruebas actualizadas.
- Pruebas de `GetCarByIdQueryHandler` ahora mockean `ICarRepository`, eliminando la dependencia a Infrastructure en la capa de casos de uso.
- Prueba de integración `should_throw_when_rental_has_no_car` ajustada para persistir cliente y reproducir correctamente el escenario de auto faltante tras desacople.
- Validación de "rental sin auto" trasladada a pruebas de UseCases, manteniendo la suite de integración alineada con restricciones de EF.
- Caso "rental sin customer" también cubierto en UseCases, eliminando el escenario inconsistente de la suite de integración.
