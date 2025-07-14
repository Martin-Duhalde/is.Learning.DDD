/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Rentals.CheckAvailability;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.Integration.Rentals;

public class CheckAvailabilityQueryHandlerTests
{
    private readonly CarRentalDbContext             /**/ _db;
    private readonly EfCarRepository                /**/ _carRepo;
    private readonly CheckAvailabilityQueryHandler  /**/ _handler;

    public CheckAvailabilityQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _carRepo = new EfCarRepository(_db);
        _handler = new CheckAvailabilityQueryHandler(_carRepo);
    }

    [Fact]
    public async Task should_return_only_available_cars_that_match_model_and_type()
    {
        // Arrange
        var startDate           /**/ = DateTime.UtcNow.AddDays(1);
        var endDate             /**/ = DateTime.UtcNow.AddDays(3);

        var matchingCar1        /**/ = new Car { Id = Guid.NewGuid(), Type = "SUV", Model = "ModelX" };
        var matchingCar2        /**/ = new Car { Id = Guid.NewGuid(), Type = "SUV", Model = "ModelX" };
        var nonMatchingCar      /**/ = new Car { Id = Guid.NewGuid(), Type = "Sedan", Model = "Another" };

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
