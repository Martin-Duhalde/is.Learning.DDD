/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Services.GetUpcoming;

namespace CarRental.Tests.Integration.Services;

public class GetUpcomingServicesQueryHandlerTests
{
    private readonly CarRentalDbContext                    /**/ _db;
    private readonly EfServiceRepository                   /**/ _serviceRepo;
    private readonly GetUpcomingServicesQueryHandler       /**/ _handler;

    public GetUpcomingServicesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _serviceRepo = new EfServiceRepository(_db);
        _handler = new GetUpcomingServicesQueryHandler(_serviceRepo);
    }

    [Fact]
    public async Task should_return_upcoming_services_with_correct_model_type_and_date()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = from.AddDays(7);

        var car1 = new Car { Id = Guid.NewGuid(), Model = "ModelA", Type = "SUV" };
        var car2 = new Car { Id = Guid.NewGuid(), Model = "ModelB", Type = "Sedan" };

        await _db.Cars.AddRangeAsync(car1, car2);

        var service1 = new Service { Id = Guid.NewGuid(), CarId = car1.Id, Date = from.AddDays(1) };
        var service2 = new Service { Id = Guid.NewGuid(), CarId = car2.Id, Date = from.AddDays(3) };
        var serviceOutsideRange = new Service { Id = Guid.NewGuid(), CarId = car1.Id, Date = from.AddDays(10) };

        await _db.Services.AddRangeAsync(service1, service2, serviceOutsideRange);
        await _db.SaveChangesAsync();

        var query = new GetUpcomingServicesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, s => s.Model == "ModelA" && s.Type == "SUV" && s.Date == service1.Date);
        Assert.Contains(result, s => s.Model == "ModelB" && s.Type == "Sedan" && s.Date == service2.Date);
        Assert.DoesNotContain(result, s => s.Date == serviceOutsideRange.Date);
    }

    [Fact]
    public async Task should_include_car_entity_with_model_and_type_in_services()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = from.AddDays(7);

        var car1 = new Car { Id = Guid.NewGuid(), Model = "ModelA", Type = "SUV" };
        var car2 = new Car { Id = Guid.NewGuid(), Model = "ModelB", Type = "Sedan" };

        await _db.Cars.AddRangeAsync(car1, car2);

        var service1 = new Service { Id = Guid.NewGuid(), CarId = car1.Id, Date = from.AddDays(1) };
        var service2 = new Service { Id = Guid.NewGuid(), CarId = car2.Id, Date = from.AddDays(3) };

        await _db.Services.AddRangeAsync(service1, service2);
        await _db.SaveChangesAsync();

        var query = new GetUpcomingServicesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        foreach (var service in result)
        {
            // Asumiendo que el resultado tiene acceso directo a Car o que el DTO tiene Model y Type
            Assert.False(string.IsNullOrEmpty(service.Model), "Service.Model should not be null or empty");
            Assert.False(string.IsNullOrEmpty(service.Type), "Service.Type should not be null or empty");

            // Opcional: si es posible, validar que fecha está en rango
            Assert.InRange(service.Date, from, to);
        }

        // Además validar que los servicios estén correctamente asociados a sus coches:
        Assert.Contains(result, s => s.Model == car1.Model && s.Type == car1.Type);
        Assert.Contains(result, s => s.Model == car2.Model && s.Type == car2.Type);
    }
}
