/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Cars.GetById;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.UseCases.Cars;

public class GetCarByIdQueryHandlerTests
{
    private readonly DbContextOptions<CarRental.Infrastructure.Databases.CarRentalDbContext> /**/ _dbOptions;
    private readonly CarRental.Infrastructure.Databases.CarRentalDbContext /**/ _db;
    private readonly GetCarByIdQueryHandler /**/ _handler;

    public GetCarByIdQueryHandlerTests()
    {
        _dbOptions = new DbContextOptionsBuilder<CarRental.Infrastructure.Databases.CarRentalDbContext>()
                        .UseInMemoryDatabase(databaseName: "CarRentalDb_" + Guid.NewGuid())
                        .Options;

        _db = new CarRental.Infrastructure.Databases.CarRentalDbContext(_dbOptions);
        _db.Database.EnsureCreated();

        _handler = new GetCarByIdQueryHandler(_db);
    }

    [Fact]
    public async Task should_return_car_when_it_exists()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var car = new Car
        {
            Id = carId,
            Model = "Test Model",
            Type = "Test Type",
            IsActive = true,
            Version = 1
        };

        await _db.Cars.AddAsync(car);
        await _db.SaveChangesAsync();

        var query = new GetCarByIdQuery(carId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(carId, result.Id);
        Assert.Equal("Test Model", result.Model);
        Assert.Equal("Test Type", result.Type);
        Assert.True(result.IsActive);
        Assert.Equal(1, result.Version);
    }

    [Fact]
    public async Task should_throw_keynotfoundexception_when_car_does_not_exist()
    {
        // Arrange
        var nonExistentCarId = Guid.NewGuid();
        var query = new GetCarByIdQuery(nonExistentCarId);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}
