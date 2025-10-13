/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Tests.Integration.TestBuilders;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Services;
using CarRental.Application.Rentals.CheckAvailability;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.Integration.Rentals;

public class CheckAvailabilityQueryHandlerTests
{
    private readonly CarRentalDbContext             /**/ _db;
    private readonly CarAvailabilityReadService     /**/ _availabilityService;
    private readonly CheckAvailabilityQueryHandler  /**/ _handler;

    public CheckAvailabilityQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _availabilityService = new CarAvailabilityReadService(_db);
        _handler = new CheckAvailabilityQueryHandler(_availabilityService);
    }

    [Fact]
    public async Task should_return_only_available_cars_that_match_model_and_type()
    {
        // Arrange
        var startDate           /**/ = DateTime.UtcNow.AddDays(1);
        var endDate             /**/ = DateTime.UtcNow.AddDays(3);

        var matchingCar1        /**/ = Car.Restore(Guid.NewGuid(), "ModelX", "SUV", isActive: true, version: 1);
        var matchingCar2        /**/ = Car.Restore(Guid.NewGuid(), "ModelX", "SUV", isActive: true, version: 1);
        var nonMatchingCar      /**/ = Car.Restore(Guid.NewGuid(), "Another", "Sedan", isActive: true, version: 1);

        await _db.Cars.AddRangeAsync(matchingCar1, matchingCar2, nonMatchingCar);

        // matchingCar2 está alquilado en el mismo rango → no debe aparecer
        var rental = new Rental
        {
            Id          /**/ = Guid.NewGuid(),
            CarId       /**/ = matchingCar2.Id,
            CustomerId  /**/ = Guid.NewGuid(),
            StartDate   /**/ = startDate,
            EndDate     /**/ = endDate
        };

        await _db.Rentals.AddAsync(rental);
        await _db.SaveChangesAsync();

        var query = new CheckAvailabilityQuery(startDate, endDate, "SUV", "ModelX");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(matchingCar1.Id    /**/ , result[0].Id);
        Assert.Equal("SUV"              /**/ , result[0].Type);
        Assert.Equal("ModelX"           /**/ , result[0].Model);
    }
}
