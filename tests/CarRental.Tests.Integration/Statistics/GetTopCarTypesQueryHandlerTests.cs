/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Statistics.GetTopCarTypes;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.Integration.Statistics;

public class GetTopCarTypesQueryHandlerTests
{
    private readonly CarRentalDbContext              /**/ _db;
    private readonly EfRentalRepository              /**/ _rentalRepo;
    private readonly EfCarRepository                 /**/ _carRepo;
    private readonly GetTopCarTypesQueryHandler      /**/ _handler;

    public GetTopCarTypesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db         /**/ = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _rentalRepo = new EfRentalRepository(_db);
        _carRepo = new EfCarRepository(_db);
        _handler = new GetTopCarTypesQueryHandler(_rentalRepo, _carRepo);
    }

    [Fact]
    public async Task should_return_top_3_car_types_with_percentages()
    {
        // Arrange
        var from = DateTime.UtcNow.AddDays(-10);
        var to = DateTime.UtcNow;

        var suv = new Car { Id = Guid.NewGuid(), Model = "X1", Type = "SUV" };
        var sedan = new Car { Id = Guid.NewGuid(), Model = "S1", Type = "Sedan" };
        var hatch = new Car { Id = Guid.NewGuid(), Model = "H1", Type = "Hatch" };
        var coupe = new Car { Id = Guid.NewGuid(), Model = "C1", Type = "Coupe" };

        await _db.Cars.AddRangeAsync(suv, sedan, hatch, coupe);

        var rentals = new List<Rental>
        {
            new() { Id = Guid.NewGuid(), CarId = suv.Id,   Car = suv,   StartDate = from.AddDays(1), EndDate = to, RentalStatus = RentalStatus.Active },
            new() { Id = Guid.NewGuid(), CarId = suv.Id,   Car = suv,   StartDate = from.AddDays(2), EndDate = to, RentalStatus = RentalStatus.Active },
            new() { Id = Guid.NewGuid(), CarId = sedan.Id, Car = sedan, StartDate = from.AddDays(3), EndDate = to, RentalStatus = RentalStatus.Active },
            new() { Id = Guid.NewGuid(), CarId = hatch.Id, Car = hatch, StartDate = from.AddDays(4), EndDate = to, RentalStatus = RentalStatus.Active },
            new() { Id = Guid.NewGuid(), CarId = coupe.Id, Car = coupe, StartDate = from.AddDays(5), EndDate = to, RentalStatus = RentalStatus.Active }
        };

        await _db.Rentals.AddRangeAsync(rentals);
        await _db.SaveChangesAsync();

        var query = new GetTopCarTypesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);

        Assert.Equal("SUV", result[0].Type);
        Assert.Equal(2, result[0].Count);
        Assert.Equal(40.0, result[0].Percentage); // 2/5

        Assert.Equal("Sedan", result[1].Type);
        Assert.Equal(1, result[1].Count);
        Assert.Equal(20.0, result[1].Percentage); // 1/5

        Assert.Equal("Hatch", result[2].Type);
        Assert.Equal(1, result[2].Count);
        Assert.Equal(20.0, result[2].Percentage);
    }
}
