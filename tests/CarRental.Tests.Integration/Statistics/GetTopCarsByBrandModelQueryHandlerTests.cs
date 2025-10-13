/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Tests.Integration.TestBuilders;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.Application.Statistics.GetTopCarsByBrandModel;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.Integration.Statistics;

public class GetTopCarsByBrandModelQueryHandlerTests
{
    private readonly CarRentalDbContext             /**/ _db;
    private readonly EfRentalRepository             /**/ _rentalRepo;
    private readonly GetTopCarsByBrandModelQueryHandler /**/ _handler;

    public GetTopCarsByBrandModelQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _rentalRepo = new EfRentalRepository(_db);
        _handler = new GetTopCarsByBrandModelQueryHandler(_rentalRepo);
    }

    [Fact]
    public async Task should_return_top_cars_by_brand_and_model()
    {
        // Arrange
        var from = DateTime.UtcNow.AddDays(-10);
        var to = DateTime.UtcNow;

        var car1 = DomainBuilder.BuildCar(model: "Model A", type: "SUV");
        var car2 = DomainBuilder.BuildCar(model: "Model B", type: "Sedan");

        await _db.Cars.AddRangeAsync(car1, car2);

        var rental1 = new Rental { Id = Guid.NewGuid(), CarId = car1.Id, Car = car1, StartDate = from.AddDays(1), EndDate = to, RentalStatus = RentalStatus.Active };
        var rental2 = new Rental { Id = Guid.NewGuid(), CarId = car1.Id, Car = car1, StartDate = from.AddDays(2), EndDate = to, RentalStatus = RentalStatus.Active };
        var rental3 = new Rental { Id = Guid.NewGuid(), CarId = car2.Id, Car = car2, StartDate = from.AddDays(3), EndDate = to, RentalStatus = RentalStatus.Active };

        await _db.Rentals.AddRangeAsync(rental1, rental2, rental3);
        await _db.SaveChangesAsync();

        var query = new GetTopCarsByBrandModelQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);

        var top1 = result.First();
        Assert.Equal("Model A", top1.Model);
        Assert.Equal("SUV", top1.Type);
        Assert.Equal(2, top1.Count);
        Assert.Equal(66.67, top1.Percentage);
    }
}
