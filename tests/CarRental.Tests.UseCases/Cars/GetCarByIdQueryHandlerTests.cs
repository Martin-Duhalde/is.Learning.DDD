/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Cars.GetById;

namespace CarRental.Tests.UseCases.Cars;

public class GetCarByIdQueryHandlerTests
{
    private readonly ICarRepository          /**/ _carRepository = Substitute.For<ICarRepository>();
    private readonly GetCarByIdQueryHandler /**/ _handler;

    public GetCarByIdQueryHandlerTests()
    {
        _handler = new GetCarByIdQueryHandler(_carRepository);
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

        _carRepository.GetActiveByIdAsync(carId, Arg.Any<CancellationToken>())
                      .Returns(car);

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
        _carRepository.GetActiveByIdAsync(nonExistentCarId, Arg.Any<CancellationToken>())
                      .Returns((Car?)null);
        var query = new GetCarByIdQuery(nonExistentCarId);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}
